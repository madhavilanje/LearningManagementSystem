using AutoMapper;
using LMS.Application.Interfaces;
using LMS.Application.RequestDataModels.Course;
using LMS.Infrastructure.Common.DataModels;
using LMS.Infrastructure.Repositories;
using LMS.Infrastructure.SqlClietInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using static LMS.Application.Common.ResponseMessageConstants;

using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static LMS.Infrastructure.Common.Generic.Enums;
using LMS.Application.RequestDataModels.User;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LMS.Application.ResponseDataModels;

namespace LMS.Application.Implementations
{
    public class EnrollmentsManager : IEnrollmentsManager
    {
        #region Private Fields

        private readonly IGenericRepository<User> userRepository;
        private readonly IGenericRepository<Course> courseRepository;
        private readonly IGenericRepository<LearningPath> learningPathRepository;
        private readonly IGenericRepository<Enrollment> enrollmentRepository;
        private readonly ILogger<IEnrollmentsManager> logger;
        private readonly IMapper mapper;

        #endregion Private Fields

        public EnrollmentsManager(IServiceProvider serviceProvider)
        {
            userRepository = serviceProvider.GetRequiredService<IGenericRepository<User>>();
            courseRepository = serviceProvider.GetRequiredService<IGenericRepository<Course>>();
            learningPathRepository = serviceProvider.GetRequiredService<IGenericRepository<LearningPath>>();
            enrollmentRepository = serviceProvider.GetRequiredService<IGenericRepository<Enrollment>>();
            logger = serviceProvider.GetRequiredService<ILogger<IEnrollmentsManager>>();
            mapper = serviceProvider.GetRequiredService<IMapper>();
        }
        #region Get

        /// <summary>
        /// This method is used to get enrollments details.
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse> GetAllEnrollmentsAsync(UserClaimDataModel claimDataModel, int? learningPathId, int? courseId)
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
            if (loggedInUserEntity.Role.Equals(UserRole.GuestUser.ToString(), StringComparison.OrdinalIgnoreCase))
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(UnprivilegedUser, claimDataModel.Email)
                    },
                    StatusCode = HttpStatusCode.Forbidden
                };

            IEnumerable<Enrollment> enrollments = new List<Enrollment>();
            if (loggedInUserEntity.Role.Equals(UserRole.Instructor.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                if (learningPathId != null && courseId != null)
                    enrollments = await enrollmentRepository.FindAllAsync(u => u.CourseId == courseId && u.LearningPathId == learningPathId);
                else
                    enrollments = await enrollmentRepository.FindAllAsync(null);
            }
            else
            {
                if (learningPathId != null && courseId != null)
                    enrollments = await enrollmentRepository.FindAllAsync(u => u.CourseId == courseId && u.LearningPathId == learningPathId && u.StudentId == loggedInUserEntity.Id);
                else
                    enrollments = await enrollmentRepository.FindAllAsync(u => u.StudentId == loggedInUserEntity.Id);
            }

            var enrollmentViewModels = new List<EnrollmentViewModel>(); // Creating a  new list to store view models

            foreach (var entity in enrollments)
            {
                var course = await courseRepository.FindAsync(u => u.Id == entity.CourseId);
                var learningPath = await learningPathRepository.FindAsync(u => u.Id == entity.LearningPathId);
                var user = await userRepository.FindAsync(u => u.Id == entity.StudentId);

                var enrollmentsViewModel = mapper.Map<EnrollmentViewModel>(entity);
                enrollmentsViewModel.CourseName = course?.Title;
                enrollmentsViewModel.StudentName = user?.Name;
                enrollmentsViewModel.LearningPathName = learningPath?.Title;
                enrollmentViewModels.Add(enrollmentsViewModel);
            }
            return new SuccessResponse<IEnumerable<EnrollmentViewModel>>
            { Data = enrollmentViewModels, StatusCode = HttpStatusCode.OK };
        }

        #endregion Get

        #region Post Operation

        /// <summary>
        /// This method is used to enroll student into a course
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse> EnrollCourseAsync(int learningPathId, int courseId, UserClaimDataModel userClaimData)
        {
            User loggedInUserEntity = await userRepository.FindAsync(u => u.Email == userClaimData.Email);
            if (loggedInUserEntity == null) // null check is not required, as this user has to be present in DB, still added validation.
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(InvalidUser, userClaimData.Email)
                    },
                    StatusCode = HttpStatusCode.Unauthorized
                };

            //If coming role is Admin then logged in user role has to be admin.
            if (loggedInUserEntity.Role.Equals(UserRole.Instructor.ToString(), StringComparison.OrdinalIgnoreCase) || loggedInUserEntity.Role.Equals(UserRole.GuestUser.ToString(), StringComparison.OrdinalIgnoreCase))
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(UnprivilegedUser, userClaimData.Email)
                    },
                    StatusCode = HttpStatusCode.Forbidden
                };

            Enrollment isCourseAlreadyEnrolled = await enrollmentRepository.FindAsync(u => u.CourseId == courseId && u.LearningPathId == learningPathId && u.StudentId == loggedInUserEntity.Id);
            if (isCourseAlreadyEnrolled != null)
            {
                var isCourseUnenrolled = await enrollmentRepository.DeleteAsync(isCourseAlreadyEnrolled);
                if (isCourseUnenrolled <= 0)
                    return new ErrorResponse<ApiErrorResponse>
                    {
                        Error = new ApiErrorResponse
                        {
                            Message = string.Format(CourseEnrollementFailed)
                        },
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                return new SuccessResponse<string> { Data = string.Format("Course unenrollment success"), StatusCode = HttpStatusCode.OK };
            }
            else
            {
                var courseEnrollmentEntity = PrepareCourseEnrollmentToRegister(courseId, learningPathId, loggedInUserEntity.Id);

                var enrollment = await enrollmentRepository.AddAsync(courseEnrollmentEntity);
                int isCourseEnrolled = await courseRepository.SaveChangesAsync();
                if (isCourseEnrolled <= 0)
                    return new ErrorResponse<ApiErrorResponse>
                    {
                        Error = new ApiErrorResponse
                        {
                            Message = string.Format(CourseEnrollementFailed)
                        },
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                return new SuccessResponse<string> { Data = string.Format(CourseEnrollmentSuccess), StatusCode = HttpStatusCode.OK };
            }
            

            
        }

        #endregion Post Operation

        #region Private Methods
        private static Enrollment PrepareCourseEnrollmentToRegister(int courseId, int learningPathId, int studentId)
        {
            return new Enrollment
            {
                CourseId = courseId,
                LearningPathId = learningPathId,
                StudentId = studentId
                //Progress = 
            };
        }

        #endregion Private Methods

    }
}
