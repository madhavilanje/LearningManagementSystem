using AutoMapper;
using static LMS.Application.Common.ResponseMessageConstants;
using LMS.Application.Interfaces;
using LMS.Infrastructure.Common.DataModels;
using LMS.Infrastructure.Repositories;
using LMS.Infrastructure.SqlClietInterfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using LMS.Application.ResponseDataModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static LMS.Infrastructure.Common.Generic.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LMS.Application.RequestDataModels.User;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LMS.Infrastructure.Common.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace LMS.Application.Implementations
{
    public class UsersManager : IUsersManager
    {
        #region Private Fields

        private IConfiguration _config;
        private readonly IGenericRepository<User> userRepository;
        private readonly ILogger<IUsersManager> logger;
        private readonly IGenericRepository<Course> courseRepository;
        private readonly IGenericRepository<LearningPath> learningPathRepository;
        private readonly IGenericRepository<Discussion> discussionRepository;
        private readonly IGenericRepository<Enrollment> enrollmentRepository;
        //private readonly AppSettings appSettings;
        private readonly IMapper mapper;

        #endregion Private Fields

        public UsersManager(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _config = configuration;
            userRepository = serviceProvider.GetRequiredService<IGenericRepository<User>>();
            courseRepository = serviceProvider.GetRequiredService<IGenericRepository<Course>>();
            learningPathRepository = serviceProvider.GetRequiredService<IGenericRepository<LearningPath>>();
            discussionRepository = serviceProvider.GetRequiredService<IGenericRepository<Discussion>>();
            logger = serviceProvider.GetRequiredService<ILogger<IUsersManager>>();
            mapper = serviceProvider.GetRequiredService<IMapper>();
            enrollmentRepository = serviceProvider.GetRequiredService<IGenericRepository<Enrollment>>();
            //appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;

        }

        #region GetOperation

        /// <summary>
        /// This method is used to get all messages
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse> GetMessageAsync( UserClaimDataModel claimDataModel)
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

            var messages = await discussionRepository.FindAllAsync(null);
            var messageViewModels = new List<MessageViewModel>();
            foreach ( var message in messages )
            {
                var messageViewModel = mapper.Map<MessageViewModel>(message);
                messageViewModel.Name = userRepository.FindAsync(u => u.Id == message.UserId).Result.Name;
                messageViewModels.Add(messageViewModel);
            }

            return new SuccessResponse<List<MessageViewModel>> { Data = messageViewModels, StatusCode = HttpStatusCode.OK };
        }

        /// <summary>
        /// This method is used to get logged in user profile.
        /// </summary>
        /// <param name="claimDataModel"></param>
        /// <returns></returns>
        public async Task<ApiResponse> GetLoggedInUserProfileAsync(UserClaimDataModel claimDataModel)
        {
            IEnumerable<User> userEntities = await userRepository.GetAsync(u => u.Email == claimDataModel.Email);
            User loggedInUserEntity = userEntities.FirstOrDefault();
            if (loggedInUserEntity == null)
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(InvalidUser, claimDataModel.Email)
                    },
                    StatusCode = HttpStatusCode.Unauthorized
                };

            // If account details were not found, return a SuccessResponse with null account data.
            return new SuccessResponse<BaseUserProfileViewModel> { Data = mapper.Map<BaseUserProfileViewModel>(loggedInUserEntity), StatusCode = HttpStatusCode.OK };
        }

        /// <summary>
        /// This method is used to get team members details.
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse> GetAllUsersAsync(UserClaimDataModel claimDataModel)
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
            IEnumerable<User> userEntities = new List<User>();
            if (loggedInUserEntity.Role.Equals(UserRole.Instructor.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                userEntities = await userRepository.FindAllAsync(null);
            }
            else
            {
                userEntities = await userRepository.FindAllAsync(u => u.Role == UserRole.Student.ToString());
            }
            return new SuccessResponse<IEnumerable<BaseUserProfileViewModel>>
            { Data = mapper.Map<IEnumerable<BaseUserProfileViewModel>>(userEntities), StatusCode = HttpStatusCode.OK };
        }

        /// <summary>
        /// This API is used to get all enrolled courses by a student
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse> GetEnrollmentsBasedOnStudentIdAsync(UserClaimDataModel claimDataModel, int studentId)
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

            if (loggedInUserEntity.Role.Equals(UserRole.GuestUser.ToString(), StringComparison.OrdinalIgnoreCase))
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(UnprivilegedUser, claimDataModel.Email)
                    },
                    StatusCode = HttpStatusCode.Forbidden
                };

            IEnumerable<Enrollment> enrollments = await enrollmentRepository.FindAllAsync(u => u.StudentId == studentId);

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

        #endregion Get Operation

        #region Post Operation

        /// <summary>
        /// This method is used to login.
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        public async Task<ApiResponse> LoginAsync(LoginRequest loginRequest)
        {
            User userEntity = await userRepository.FindAsync(u => u.Email == loginRequest.Email && u.Password == loginRequest.Password);
            if (userEntity == null)
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(LoginFailed, loginRequest.Email)
                    },
                    StatusCode = HttpStatusCode.Unauthorized
                };

            var token = GenerateToken(userEntity);

            BaseUserProfileViewModel baseUserProfile = mapper.Map<BaseUserProfileViewModel>(userEntity);
            baseUserProfile.Token = token;

            return new SuccessResponse<BaseUserProfileViewModel> { Data = baseUserProfile, StatusCode = HttpStatusCode.OK };
        }

        /// <summary>
        /// This method is used to send message
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse> SendMessageAsync(MessageRequest request, UserClaimDataModel claimDataModel)
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

            var messageEntity = PrepareMessageEntityToRegister(request, loggedInUserEntity);
            var message = await discussionRepository.AddAsync(messageEntity);
            int isMessagePosted = await userRepository.SaveChangesAsync();
            if (isMessagePosted <= 0)
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(MessagePostingFailed, message.Message)
                    },
                    StatusCode = HttpStatusCode.InternalServerError
                };

            return new SuccessResponse<string> { Data = string.Format(MessagePostingSuccess, message.Message), StatusCode = HttpStatusCode.OK };
        }

        /// <summary>
        /// This method is used to add new user
        /// </summary>
        /// <param name="userProfile"></param>
        /// <returns></returns>
        public async Task<ApiResponse> RegisterUser(UserRequest userProfile)
        {

            User doesUserExist = await userRepository.FindAsync(u => u.Email == userProfile.Email);
            if (doesUserExist != null)
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(UserAlreadyExist, userProfile.Email)
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };

            var userEntity = PrepareUserEntityToRegister(userProfile);
            var user = await userRepository.AddAsync(userEntity);
            int createUser = await userRepository.SaveChangesAsync();
            if (createUser <= 0)
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(UserCreationFailed, userProfile.Email)
                    },
                    StatusCode = HttpStatusCode.InternalServerError
                };

            return new SuccessResponse<string> { Data = string.Format(UserCreationSuccess, userProfile.Email), StatusCode = HttpStatusCode.OK };
        }

        #endregion Post Operation

        #region Put Operation

        /// <summary>
        /// This method is used to assign role to the requested user.
        /// </summary>
        /// <param name="assignRoleRequest"></param>
        /// <returns></returns>
        public async Task<ApiResponse> AssignRoleAsync(AssignRoleRequest assignRole, UserClaimDataModel claimDataModel)
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
            User userEntity = await userRepository.FindAsync(u => u.Email == assignRole.Email);
            if (userEntity == null) //if user details not found.
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(InvalidUser, assignRole.Email)
                    },
                    StatusCode = HttpStatusCode.NotFound
                };

            if (!loggedInUserEntity.Role.Equals(UserRole.Instructor.ToString(), StringComparison.OrdinalIgnoreCase))
                return new ErrorResponse<ApiErrorResponse>//Only Instructor role can assign role to user.
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(UnprivilegedUser, claimDataModel.Email)
                    },
                    StatusCode = HttpStatusCode.Forbidden
                };

            userEntity.Role = assignRole.Role;
            userEntity = await userRepository.UpdateAsync(userEntity);
            int updatedUser = await userRepository.SaveChangesAsync();
            if (updatedUser <= 0)
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(UserUpdationFailed, assignRole.Email)
                    },
                    StatusCode = HttpStatusCode.InternalServerError
                };

            return new SuccessResponse<string> { Data = string.Format(UserUpdationSuccess, assignRole.Email), StatusCode = HttpStatusCode.OK };
        }

        #endregion Put Operation

        #region Delete Operation
        /// <summary>
        /// This method is used to remove the user from the database.
        /// </summary>
        /// <param name="removeUser"></param>
        /// <returns></returns>
        public async Task<ApiResponse> RemoveUserAsync(RemoveUserRequest removeUser, UserClaimDataModel claimDataModel)
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
            if (loggedInUserEntity.Role.Equals(UserRole.Student.ToString(), StringComparison.OrdinalIgnoreCase))
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(UnprivilegedUser, claimDataModel.Email)
                    },
                    StatusCode = HttpStatusCode.Forbidden
                };

            // Retrieving the user details from the DB
            User userEntity = await userRepository.FindAsync(u => u.Email == removeUser.Email);
            if (userEntity == null) //if user details not found.
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(InvalidUser, removeUser.Email)
                    },
                    StatusCode = HttpStatusCode.NotFound
                };
            // Firstly remove the enrollments made by user
            var enrollments = await enrollmentRepository.FindAllAsync(u => u.StudentId == userEntity.Id);
            if (!enrollments.IsNullOrEmpty())
            {
                foreach (var enrollment in enrollments)
                {
                    await enrollmentRepository.DeleteAsync(enrollment);
                }
            }

            // Now remove the member from the team
            int deletedTeamMember = await userRepository.DeleteAsync(userEntity);
            if (deletedTeamMember <= 0)
                return new ErrorResponse<ApiErrorResponse>
                {
                    Error = new ApiErrorResponse
                    {
                        Message = string.Format(UserDeletionFailed, removeUser.Email)
                    },
                    StatusCode = HttpStatusCode.InternalServerError
                };

            return new SuccessResponse<string> { Data = string.Format(UserDeletionSuccess, removeUser.Email), StatusCode = HttpStatusCode.OK };

        }

        #endregion Delete Operation

        #region Private Methods
        private string GenerateToken(User userEntity)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userEntity.Name),
                new Claim(ClaimTypes.Email, userEntity.Email)
            };
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private static Discussion PrepareMessageEntityToRegister(MessageRequest message, User loggedInUserEntity)
        {
            return new Discussion
            {
                Message = message.Message,
                CreatedAt = DateTime.Now,
                UserId = loggedInUserEntity.Id,
            };
        }
        private static User PrepareUserEntityToRegister(UserRequest userProfile)
        {
            return new User
            {
                Name = userProfile.Name.Trim(),
                Password = userProfile.Password.Trim(),
                Email = userProfile.Email.Trim(),
                Role = userProfile.Role,
            };
        }


        #endregion Private Methods


    }
}
