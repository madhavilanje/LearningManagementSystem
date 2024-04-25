using AutoMapper;
using LMS.Application.Interfaces;
using LMS.Application.RequestDataModels.Course;
using LMS.Application.ResponseDataModels;
using LMS.Infrastructure.Common.DataModels;
using LMS.Infrastructure.Repositories;
using static LMS.Application.Common.ResponseMessageConstants;
using LMS.Infrastructure.SqlClietInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static LMS.Infrastructure.Common.Generic.Enums;
using LMS.Application.RequestDataModels.User;
using LMS.Application.RequestDataModels.LearningPath;
using Microsoft.IdentityModel.Tokens;

namespace LMS.Application.Implementations
{
    public class LearningPathsManager : ILearningPathsManager
    {

        #region Private Fields

        private readonly IGenericRepository<User> userRepository;
        private readonly IGenericRepository<Course> courseRepository;
        private readonly IGenericRepository<Enrollment> enrollmentRepository;
        private readonly IGenericRepository<LearningPath> learningPathRepository;
        private readonly ILogger<ILearningPathsManager> logger;
        private readonly IMapper mapper;

        #endregion Private Fields

        public LearningPathsManager(IServiceProvider serviceProvider)
        {
            userRepository = serviceProvider.GetRequiredService<IGenericRepository<User>>();
            courseRepository = serviceProvider.GetRequiredService<IGenericRepository<Course>>();
            learningPathRepository = serviceProvider.GetRequiredService<IGenericRepository<LearningPath>>();
            enrollmentRepository = serviceProvider.GetRequiredService<IGenericRepository<Enrollment>>();
            logger = serviceProvider.GetRequiredService<ILogger<ILearningPathsManager>>();
            mapper = serviceProvider.GetRequiredService<IMapper>();
        }

        #region Get Operation

        /// <summary>
        /// This method is used to get all courses
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse> GetAllLearningPathsAsync()
        {
            IEnumerable<LearningPath> learningPathEntities = await learningPathRepository.FindAllAsync(null);

            return new SuccessResponse<IEnumerable<LearningPathViewModel>>
            { Data = mapper.Map<IEnumerable<LearningPathViewModel>>(learningPathEntities), StatusCode = HttpStatusCode.OK };

        }

        #endregion Get operation

        #region Post Operation

        /// <summary>
        /// This method is used to add new learning path
        /// </summary>
        /// <param name="userProfile"></param>
        /// <returns></returns>
        public async Task<ApiResponse> AddLearningPathAsync(LearningPathRequest request, UserClaimDataModel claimDataModel)
        {
            User loggedInUserEntity = await userRepository.FindAsync(u => u.Email == claimDataModel.Email);
            if (loggedInUserEntity == null) // null check is not required, as this user has to be present in DB, still added validation.
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(InvalidUser, claimDataModel.Email)
                    },
                    StatusCode = HttpStatusCode.Unauthorized
                };

            //If coming role is Admin then logged in user role has to be admin.
            if (!loggedInUserEntity.Role.Equals(UserRole.Student.ToString(), StringComparison.OrdinalIgnoreCase) && loggedInUserEntity.Role.Equals(UserRole.GuestUser.ToString(), StringComparison.OrdinalIgnoreCase))
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(UnprivilegedUser, claimDataModel.Email)
                    },
                    StatusCode = HttpStatusCode.Forbidden
                };

            LearningPath doesLearningPathAlreadyExist = await learningPathRepository.FindAsync(u => u.Title == request.Title);
            if (doesLearningPathAlreadyExist != null)
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(LearningPathAlreadyExist, request.Title)
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
            var learningPathEntity = PrepareLaerningPathEntityToRegister(request, loggedInUserEntity);
            var course = await learningPathRepository.AddAsync(learningPathEntity);
            int isCourseCreated = await learningPathRepository.SaveChangesAsync();
            if (isCourseCreated <= 0)
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(LearningPathCreationFailed, request.Title)
                    },
                    StatusCode = HttpStatusCode.InternalServerError
                };

            return new SuccessResponse<string> { Data = string.Format(LearningPathCreationSuccess, request.Title), StatusCode = HttpStatusCode.OK };
        }

        #endregion Post Operation

        #region Delete Operation

        /// <summary>
        /// This method is used to remove the learning path from db
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse> RemoveLearningPathAsync(UserClaimDataModel claimDataModel, int id)
        {
            User loggedInUserEntity = await userRepository.FindAsync(u => u.Email == claimDataModel.Email);
            if (loggedInUserEntity == null) // null check is not required, as this user has to be present in DB, still added validation.
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(InvalidUser, claimDataModel.Email)
                    },
                    StatusCode = HttpStatusCode.Unauthorized
                };

            // Check if the logged in user is a student then that user cannot delete any of the other users
            if (loggedInUserEntity.Role.Equals(UserRole.Student.ToString(), StringComparison.OrdinalIgnoreCase) || loggedInUserEntity.Role.Equals(UserRole.GuestUser.ToString(), StringComparison.OrdinalIgnoreCase))
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(UnprivilegedUser, claimDataModel.Email)
                    },
                    StatusCode = HttpStatusCode.Forbidden
                };

            // Retrieving the learning path details from the DB
            LearningPath learningPathEntity = await learningPathRepository.FindAsync(u => u.Id == id);
            if (learningPathEntity == null) //if user details not found.
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(InvalidLearningPath, id)
                    },
                    StatusCode = HttpStatusCode.NotFound
                };
            var enrollments = await enrollmentRepository.FindAllAsync(e => e.LearningPathId == id);
            if (!enrollments.IsNullOrEmpty())
            {
                foreach (var enrollment in enrollments)
                {
                    await enrollmentRepository.DeleteAsync(enrollment);
                }
            }

            var courses = await courseRepository.FindAllAsync(e => e.LearningPathId == id);
            if (!courses.IsNullOrEmpty())
            {
                foreach (var course in courses)
                {
                    await courseRepository.DeleteAsync(course);
                }
            }

            // Now remove the learning path from db 
            int deletedCourse = await learningPathRepository.DeleteAsync(learningPathEntity);
            if (deletedCourse <= 0)
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(LearningPathDeletionFailed, id)
                    },
                    StatusCode = HttpStatusCode.InternalServerError
                };

            return new SuccessResponse<string> { Data = string.Format(LearningPathDeletionSuccess, id), StatusCode = HttpStatusCode.OK };

        }

        #endregion Delete Operation

        #region Course

        #region Get Operation

        /// <summary>
        /// This method is used to get all courses based on learningPath
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse> GetAllCoursesAsync(int learningPathId)
        {
            IEnumerable<Course> courseEntities = await courseRepository.FindAllAsync(u => u.LearningPathId == learningPathId);

            List<CourseViewModel> courses = new List<CourseViewModel>();
            foreach (var course in courseEntities)
            {
                var instructor = await userRepository.FindAsync(u => u.Id == course.InstructorId);
                var courseViewModel = mapper.Map<CourseViewModel>(course);
                courseViewModel.InstructorName = instructor?.Name;

                courses.Add(courseViewModel);
            }

            return new SuccessResponse<IEnumerable<CourseViewModel>>
            { Data = courses, StatusCode = HttpStatusCode.OK };

        }

        #endregion Get operation

        #region Post Operation

        /// <summary>
        /// This method is used to add new course
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse> AddCourseAsync(CourseRequest courseRequest, UserClaimDataModel claimDataModel, int learningPathId)
        {
            User loggedInUserEntity = await userRepository.FindAsync(u => u.Email == claimDataModel.Email);
            if (loggedInUserEntity == null) // null check is not required, as this user has to be present in DB, still added validation.
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(InvalidUser, claimDataModel.Email)
                    },
                    StatusCode = HttpStatusCode.Unauthorized
                };

            //If coming role is Admin then logged in user role has to be admin.
            if (!loggedInUserEntity.Role.Equals(UserRole.Student.ToString(), StringComparison.OrdinalIgnoreCase) && loggedInUserEntity.Role.Equals(UserRole.GuestUser.ToString(), StringComparison.OrdinalIgnoreCase))
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(UnprivilegedUser, claimDataModel.Email)
                    },
                    StatusCode = HttpStatusCode.Forbidden
                };

            Course isCourseAlreadyExist = await courseRepository.FindAsync(u => u.Title == courseRequest.Title && u.LearningPathId == learningPathId);
            if (isCourseAlreadyExist != null)
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(CourseAlreadyExist, courseRequest.Title)
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
            var courseEntity = PrepareCourseEntityToRegister(courseRequest, loggedInUserEntity, learningPathId);
            var course = await courseRepository.AddAsync(courseEntity);
            int isCourseCreated = await courseRepository.SaveChangesAsync();
            if (isCourseCreated <= 0)
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(CourseCreationFailed, courseRequest.Title)
                    },
                    StatusCode = HttpStatusCode.InternalServerError
                };

            return new SuccessResponse<string> { Data = string.Format(CourseCreationSuccess, courseRequest.Title), StatusCode = HttpStatusCode.OK };
        }

        #endregion Post Operation

        
        #region Delete Operation

        /// <summary>
        /// This method is used to remove the course from db
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse> RemoveCourseAsync(UserClaimDataModel claimDataModel, int learningPathId, int courseId)
        {
            User loggedInUserEntity = await userRepository.FindAsync(u => u.Email == claimDataModel.Email);
            if (loggedInUserEntity == null) // null check is not required, as this user has to be present in DB, still added validation.
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(InvalidUser, claimDataModel.Email)
                    },
                    StatusCode = HttpStatusCode.Unauthorized
                };

            // Check if the logged in user is a student then that user cannot delete any of the other users
            if (loggedInUserEntity.Role.Equals(UserRole.Student.ToString(), StringComparison.OrdinalIgnoreCase) || loggedInUserEntity.Role.Equals(UserRole.GuestUser.ToString(), StringComparison.OrdinalIgnoreCase))
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(UnprivilegedUser, claimDataModel.Email)
                    },
                    StatusCode = HttpStatusCode.Forbidden
                };

            // Retrieving the course details from the DB
            Course courseEntity = await courseRepository.FindAsync(u => u.Id == courseId);
            if (courseEntity == null) //if user details not found.
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(InvalidCourse, courseId)
                    },
                    StatusCode = HttpStatusCode.NotFound
                };
            var enrollments = await enrollmentRepository.FindAllAsync(e => e.CourseId == courseId);
            if (!enrollments.IsNullOrEmpty())
            {
                foreach (var enrollment in enrollments)
                {
                    await enrollmentRepository.DeleteAsync(enrollment);
                }
            }
            // Now remove the course from the learning path
            int deletedCourse = await courseRepository.DeleteAsync(courseEntity);
            if (deletedCourse <= 0)
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(CourseDeletionFailed, courseId)
                    },
                    StatusCode = HttpStatusCode.InternalServerError
                };

            return new SuccessResponse<string> { Data = string.Format(CourseDeletionSuccess, courseId), StatusCode = HttpStatusCode.OK };

        }

        #endregion Delete Operation

        #endregion Course

        #region Private Methods

        private static LearningPath PrepareLaerningPathEntityToRegister(LearningPathRequest request, User adminEntity)
        {
            return new LearningPath
            {
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                InstructorId = adminEntity.Id,
            };
        }


        private static Course PrepareCourseEntityToRegister(CourseRequest courseRequest, User adminEntity, int learningPathId)
        {
            return new Course
            {
                Title = courseRequest.Title.Trim(),
                Description = courseRequest.Description.Trim(),
                ContentUrl = courseRequest.ContentUrl.Trim(),
                InstructorId = adminEntity.Id,
                LearningPathId = learningPathId,
            };
        }


        #endregion Private Methods

    }

}
