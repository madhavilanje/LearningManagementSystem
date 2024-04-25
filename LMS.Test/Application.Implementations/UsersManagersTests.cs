using AutoMapper;
using Microsoft.Extensions.Configuration;
using LMS.Application.Implementations;
using LMS.Application.Interfaces;
using LMS.Infrastructure.Common.Generic;
using LMS.Application.RequestDataModels;
using LMS.Application.RequestDataModels.User;
using LMS.Infrastructure.Common.DataModels;
using LMS.Infrastructure.Common.Interfaces;
using LMS.Infrastructure.Repositories;
using LMS.Infrastructure.SqlClietInterfaces;
using LMS.UserManagement.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using LMS.Application.ResponseDataModels;
using static LMS.Infrastructure.Common.Generic.Enums;
using LMS.Infrastructure.Common.Config;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace LMS.Test.Implementations
{
    public class UsersManagerTests
    {
        #region Private Fields

        private UsersManager manager;
        private Mock<IGenericRepository<User>> mockUserRepository;
        private Mock<IGenericRepository<Course>> mockCourseRepository;
        private Mock<IGenericRepository<LearningPath>> mockLearningPathRepository;
        private Mock<IGenericRepository<Enrollment>> mockEnrollmentRepository;
        private Mock<IMapper> mockMapper;
        private Mock<IConfiguration> mockConfiguration;
        private Mock<ILogger<IUsersManager>> mockLogger;

        #endregion

        [SetUp]
        public void Setup()
        {
            var company = new User { Id = 1, Name = "Test", Email = "sample@gmail.com" };
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockUserRepository = new Mock<IGenericRepository<User>>();
            mockCourseRepository = new Mock<IGenericRepository<Course>>();
            mockLearningPathRepository = new Mock<IGenericRepository<LearningPath>>();
            mockEnrollmentRepository = new Mock<IGenericRepository<Enrollment>>();
            mockMapper = new Mock<IMapper>();
            mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x["Jwt:Key"]).Returns("secret_key");
            mockConfiguration.SetupGet(x => x["Jwt:Issuer"]).Returns("issuer");
            mockConfiguration.SetupGet(x => x["Jwt:Audience"]).Returns("audience");

            mockLogger = new Mock<ILogger<IUsersManager>>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IGenericRepository<User>))).Returns(mockUserRepository.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(IGenericRepository<Course>))).Returns(mockCourseRepository.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(IGenericRepository<LearningPath>))).Returns(mockLearningPathRepository.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(IGenericRepository<Enrollment>))).Returns(mockEnrollmentRepository.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(IMapper))).Returns(mockMapper.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(IConfiguration))).Returns(mockConfiguration.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<IUsersManager>))).Returns(mockLogger.Object);

            manager = new UsersManager(mockServiceProvider.Object, mockConfiguration.Object);
        }

        #region GetAllUsersAsync

        [Test]
        public async Task GetAllUsersAsync_WhenCalled_Returns__SuccessResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Instructor.ToString(),
                Id = 1,
                Courses = new List<Course>
                { new Course { Id = 1, Title = "", Description = "", ContentUrl = "", InstructorId = 1, LearningPathId = 1 } },

                LearningPaths = new List<LearningPath>
                {
                    new LearningPath {Id = 1, Title = "", Description = " ", InstructorId =  1} },

                Enrollments = new List<Enrollment>
                {
                    new Enrollment { Id = 1, CourseId = 1, LearningPathId = 1, StudentId = 1, Progress = 0}
                }
            };
            var profile = new BaseUserProfileViewModel
            {
                Name = claimDataModel.Name,
                Email = claimDataModel.Email,
                Role = "",
                Id = 1,
                Token = ""
            };
            var users = new List<User>
                { new User { Id = 1 } };

            var successResponse = new SuccessResponse<IEnumerable<BaseUserProfileViewModel>>
            {
                StatusCode = HttpStatusCode.OK
            };

            mockUserRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(loggedInUserEntity);

            mockUserRepository.Setup(repo => repo.FindAllAsync(null))
                .ReturnsAsync(users);

            // Act
            var response = await manager.GetAllUsersAsync(claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
        }

        [Test]
        public async Task GetAllUsersAsync_WhenCalled_Returns_SuccessResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Student.ToString(),
                Id = 1,

                Courses = new List<Course>
                { new Course { Id = 1, Title = "", Description = "", ContentUrl = "", InstructorId = 1, LearningPathId = 1 } },

                LearningPaths = new List<LearningPath>
                {
                    new LearningPath {Id = 1, Title = "", Description = " ", InstructorId =  1} },

                Enrollments = new List<Enrollment>
                {
                    new Enrollment { Id = 1, CourseId = 1, LearningPathId = 1, StudentId = 1, Progress = 0}
                }
            };

            var users = new List<User>
                { new User { Id = 1 } };

            var successResponse = new SuccessResponse<IEnumerable<BaseUserProfileViewModel>>
            {
                StatusCode = HttpStatusCode.OK
            };

            mockUserRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(loggedInUserEntity);

            mockUserRepository.Setup(repo => repo.FindAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(users);

            // Act
            var response = await manager.GetAllUsersAsync(claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
        }

        [Test]
        public async Task GetAllUsersAsync_WhenCalled_Returns_UnauthorizedResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };

            var users = new List<User>
                { new User { Id = 1 } };

            var errorResponse = new ErrorResponse<ApiErrorResponse>
            {
                StatusCode = HttpStatusCode.Unauthorized
            };

            // Act
            var response = await manager.GetAllUsersAsync(claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(errorResponse.StatusCode));
        }

        #endregion GetAllUsersAsync

        #region GetEnrollmentsBasedOnStudentIdAsync

        [Test]
        public async Task GetEnrollmentsBasedOnStudentIdAsync_WhenCalled_Returns_SuccessResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Student.ToString(),
                Id = 1,

                Courses = new List<Course>
                { new Course { Id = 1, Title = "", Description = "", ContentUrl = "", InstructorId = 1, LearningPathId = 1 } },

                LearningPaths = new List<LearningPath>
                {
                    new LearningPath {Id = 1, Title = "", Description = " ", InstructorId =  1} },

                Enrollments = new List<Enrollment>
                {
                    new Enrollment { Id = 1, CourseId = 1, LearningPathId = 1, StudentId = 1, Progress = 0}
                }
            };

            var course = new Course { Id = 1, Title = "", Description = "", ContentUrl = "", InstructorId = 1, LearningPathId = 1 };

            var learningPath = new LearningPath { Id = 1, Title = "", Description = "" };

            var users = new List<User>
                { new User { Id = 1 } };

            var successResponse = new SuccessResponse<IEnumerable<BaseUserProfileViewModel>>
            {
                StatusCode = HttpStatusCode.OK
            };

            mockUserRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(loggedInUserEntity);

            mockUserRepository.Setup(repo => repo.FindAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(users);

            mockCourseRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Course, bool>>>()))
                .ReturnsAsync(course);

            mockLearningPathRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<LearningPath, bool>>>()))
                .ReturnsAsync(learningPath);

            // Act
            var response = await manager.GetAllUsersAsync(claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
        }

        [Test]
        public async Task GetEnrollmentsBasedOnStudentIdAsync_WhenCalled_Returns_UnauthorisedResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };

            var course = new Course { Id = 1, Title = "", Description = "", ContentUrl = "", InstructorId = 1, LearningPathId = 1 };

            var learningPath = new LearningPath { Id = 1, Title = "", Description = "" };

            var users = new List<User>
                { new User { Id = 1 } };

            var errorResponse = new ErrorResponse<ApiErrorResponse>
            {
                StatusCode = HttpStatusCode.Unauthorized
            };
            int id = 1;

            // Act
            var response = await manager.GetEnrollmentsBasedOnStudentIdAsync(claimDataModel, id);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(errorResponse.StatusCode));
        }

        [Test]
        public async Task GetEnrollmentsBasedOnStudentIdAsync_WhenCalled_Returns_ForbiddenResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.GuestUser.ToString(),
                Id = 1,
            };

            var course = new Course { Id = 1, Title = "", Description = "", ContentUrl = "", InstructorId = 1, LearningPathId = 1 };

            var learningPath = new LearningPath { Id = 1, Title = "", Description = "" };

            var users = new List<User> { new User { Id = 1 } };

            var successResponse = new SuccessResponse<IEnumerable<BaseUserProfileViewModel>>
            {
                StatusCode = HttpStatusCode.Forbidden
            };
            int id = 1;
            mockUserRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(loggedInUserEntity);

            // Act
            var response = await manager.GetEnrollmentsBasedOnStudentIdAsync(claimDataModel, id);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
        }

        #endregion GetEnrollmentsBasedOnStudentIdAsync

        #region LoginAsync

        /*        [Test]
                public async Task LoginAsync_WhenCalled_Returns_SuccessResponse()
                {
                    // Arrange
                    var claimDataModel = new UserClaimDataModel { Email = "test@example.com", Name = "Madhavi", Username = "mlanje" };
                    var loggedInUserEntity = new User
                    {
                        Email = claimDataModel.Email,
                        Name = claimDataModel.Name,
                        Role = UserRole.Student.ToString(),
                        Id = 1,

                        Courses = new List<Course>
                        { new Course { Id = 1, Title = "", Description = "", ContentUrl = "", InstructorId = 1, LearningPathId = 1 } },

                        LearningPaths = new List<LearningPath>
                        {
                            new LearningPath {Id = 1, Title = "", Description = " ", InstructorId =  1} },

                        Enrollments = new List<Enrollment>
                        {
                            new Enrollment { Id = 1, CourseId = 1, LearningPathId = 1, StudentId = 1, Progress = 0}
                        }
                    };

                    LoginRequest login = new LoginRequest()
                    {
                        Email = "madhavi9623@gmail.com",
                        Password = "Pass@123"
                    };
                    var successResponse = new SuccessResponse<IEnumerable<BaseUserProfileViewModel>>
                    {
                        StatusCode = HttpStatusCode.OK
                    };

                    mockUserRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
                        .ReturnsAsync(loggedInUserEntity);

                    // Act
                    var response = await manager.LoginAsync(login);

                    // Assert
                    Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
                }*/

        [Test]
        public async Task LoginAsync_WhenCalled_Returns_Unauthorised_Response()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com", Name = "Madhavi", Username = "mlanje" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Student.ToString(),
                Id = 1,
            };

            LoginRequest login = new LoginRequest()
            {
                Email = "madhavi9623@gmail.com",
                Password = "Pass@123"
            };
            var errorResponse = new ErrorResponse<string>
            {
                StatusCode = HttpStatusCode.Unauthorized
            };

            // Act
            var response = await manager.LoginAsync(login);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(errorResponse.StatusCode));
        }

        #endregion LoginAsync


        #region RegisterUser

        [Test]
        public async Task RegisterUser_WhenCalled_Returns_SuccessResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Student.ToString(),
                Id = 1
            };

            var guest = new GuestUserConfig
            {
                GuestUserKey = ""
            };

            var appSettings = new AppSettings
            {
                jwt = new Jwt
                {
                    Audience = "",
                    Key = "",
                    Issuer = ""
                }
            };

            var userProfile = new UserRequest
            {
                Name = "",
                Email = "",
                Password = "",
                Role = ""
            };

            var successResponse = new SuccessResponse<IEnumerable<BaseUserProfileViewModel>>
            {
                StatusCode = HttpStatusCode.OK
            };

            mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(loggedInUserEntity);

            mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var response = await manager.RegisterUser(userProfile);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
        }

        [Test]
        public async Task RegisterUser_WhenCalled_Returns_BadRequest_Response()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Student.ToString(),
                Id = 1
            };
            var userProfile = new UserRequest
            {
                Name = "",
                Email = "test@example.com",
                Password = "",
                Role = ""
            };
            var successResponse = new ErrorResponse<string>
            {
                StatusCode = HttpStatusCode.BadRequest
            };
            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == userProfile.Email))
                .ReturnsAsync(loggedInUserEntity);
            // Act
            var response = await manager.RegisterUser(userProfile);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
        }

        [Test]
        public async Task RegisterUser_WhenCalled_Returns_InternalServerErrorResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Student.ToString(),
                Id = 1
            };

            var userProfile = new UserRequest
            {
                Name = "",
                Email = "",
                Password = "",
                Role = ""
            };

            var errorResponse = new ErrorResponse<ApiErrorResponse>
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(loggedInUserEntity);

            mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var response = await manager.RegisterUser(userProfile);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(errorResponse.StatusCode));
        }

        #endregion RegisterUser

        #region AssignRoleAsync

        [Test]
        public async Task AssignRoleAsync_WhenCalled_Returns_SuccessResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Instructor.ToString(),
                Id = 1
            };

            var role = new AssignRoleRequest
            {
                Email = "",
                Role = "Instructor"
            };

            var successResponse = new SuccessResponse<IEnumerable<BaseUserProfileViewModel>>
            {
                StatusCode = HttpStatusCode.OK
            };

            mockUserRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(loggedInUserEntity);
            mockUserRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(loggedInUserEntity);

            mockUserRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(loggedInUserEntity);
            mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var response = await manager.AssignRoleAsync(role, claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
        }

        [Test]
        public async Task AssignRoleAsync_WhenCalled_Returns_UnauthorizedResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" , Name = " ", JwtToken = "", Username = " "};
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Instructor.ToString(),
                Id = 1
            };

            var role = new AssignRoleRequest
            {
                Email = "",
                Role = "Instructor"
            };

            var errorResponse = new ErrorResponse<ApiErrorResponse>
            {
                StatusCode = HttpStatusCode.Unauthorized
            };

            // Act
            var response = await manager.AssignRoleAsync(role, claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(errorResponse.StatusCode));
        }

        [Test]
        public async Task AssignRoleAsync_WhenCalled_Returns_NotFound_Response()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Instructor.ToString(),
                Id = 1
            };

            var role = new AssignRoleRequest
            {
                Email = "",
                Role = "Instructor"
            };

            var errorResponse = new ErrorResponse<ApiErrorResponse>
            {
                StatusCode = HttpStatusCode.NotFound
            };

            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == claimDataModel.Email))
                .ReturnsAsync(loggedInUserEntity);

            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == role.Email))
                 .ReturnsAsync((User)null);
            // Act
            var response = await manager.AssignRoleAsync(role, claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(errorResponse.StatusCode));
        }

        [Test]
        public async Task AssignRoleAsync_WhenCalled_Returns_ForbiddenResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Student.ToString(),
                Id = 1
            };

            var role = new AssignRoleRequest
            {
                Email = "",
                Role = "Instructor"
            };

            var errorResponse = new ErrorResponse<ApiErrorResponse>
            {
                StatusCode = HttpStatusCode.Forbidden
            };

            mockUserRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(loggedInUserEntity);
            mockUserRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(loggedInUserEntity);

            mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var response = await manager.AssignRoleAsync(role, claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(errorResponse.StatusCode));
        }

        [Test]
        public async Task AssignRoleAsync_WhenCalled_Returns_InternalServerErrorResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Instructor.ToString(),
                Id = 1
            };

            var role = new AssignRoleRequest
            {
                Email = "",
                Role = "Instructor"
            };

            var errorResponse = new ErrorResponse<ApiErrorResponse>
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            mockUserRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(loggedInUserEntity);
            mockUserRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(loggedInUserEntity);

            mockUserRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(loggedInUserEntity);
            mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var response = await manager.AssignRoleAsync(role, claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(errorResponse.StatusCode));
        }

        #endregion AssignRoleAsync

        #region RemoveUserAsync

        [Test]
        public async Task RemoveUserAsync_WhenCalled_Returns_SuccessResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Instructor.ToString(),
                Id = 1
            };

            var remove = new RemoveUserRequest
            {
                Email = "test@example.com",
            };

            var successResponse = new SuccessResponse<string>
            {
                StatusCode = HttpStatusCode.OK
            };

            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == claimDataModel.Email))
                .ReturnsAsync(loggedInUserEntity);
            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == remove.Email))
                .ReturnsAsync(loggedInUserEntity);

            mockUserRepository.Setup(repo => repo.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(1);
            mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var response = await manager.RemoveUserAsync(remove, claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
        }

        [Test]
        public async Task RemoveUserAsync_WhenCalled_Returns_Unauthorised_Response()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Instructor.ToString(),
                Id = 1
            };

            var remove = new RemoveUserRequest
            {
                Email = "test@example.com",
            };

            var errorResponse = new ErrorResponse<ApiErrorResponse>
            {
                StatusCode = HttpStatusCode.Unauthorized
            };

            // Act
            var response = await manager.RemoveUserAsync(remove, claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(errorResponse.StatusCode));
        }

        [Test]
        public async Task RemoveUserAsync_WhenCalled_Returns_ForbiddenResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Student.ToString(),
                Id = 1
            };

            var remove = new RemoveUserRequest
            {
                Email = "test@example.com",
            };

            var successResponse = new SuccessResponse<string>
            {
                StatusCode = HttpStatusCode.Forbidden
            };

            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == claimDataModel.Email))
                .ReturnsAsync(loggedInUserEntity);

            // Act
            var response = await manager.RemoveUserAsync(remove, claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
        }

        [Test]
        public async Task RemoveUserAsync_WhenCalled_Returns_NotFoundResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Instructor.ToString(),
                Id = 1
            };

            var remove = new RemoveUserRequest
            {
                Email = "test@example.com",
            };

            var errorResponse = new ErrorResponse<ApiErrorResponse>
            {
                StatusCode = HttpStatusCode.NotFound
            };

            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == claimDataModel.Email))
                .ReturnsAsync(loggedInUserEntity);
            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == remove.Email))
                .ReturnsAsync((User)null);


            // Act
            var response = await manager.RemoveUserAsync(remove, claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(errorResponse.StatusCode));
        }

        [Test]
        public async Task RemoveUserAsync_WhenCalled_Returns_InternalServerErrorResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.Instructor.ToString(),
                Id = 1
            };

            var profile = new BaseUserProfileViewModel
            {
                Name = claimDataModel.Name,
                Email = claimDataModel.Email,
                Role = "",
                Id = 1,
                Token = ""
            };

            var course = new CourseViewModel
            {
                ContentUrl = "",
                Description = "",
                Id = 1,
                Title = "",
                InstructorName =""
            };

            var enrollment = new EnrollmentViewModel
            {
                CourseName = "",
                Id = 8,
                LearningPathName = "",
                Progress = "a",
                StudentName = "a"
            };

            var learningPath = new LearningPathViewModel
            {
                Description = "",
                Id =1,
                Title = ""
            };

            var remove = new RemoveUserRequest
            {
                Email = "test@example.com",
            };

            var successResponse = new ErrorResponse<string>
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == claimDataModel.Email))
                .ReturnsAsync(loggedInUserEntity);
            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == remove.Email))
                .ReturnsAsync(loggedInUserEntity);

            mockUserRepository.Setup(repo => repo.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(0);
            mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var response = await manager.RemoveUserAsync(remove, claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
        }

        #endregion RemoveUserAsync

    }
}