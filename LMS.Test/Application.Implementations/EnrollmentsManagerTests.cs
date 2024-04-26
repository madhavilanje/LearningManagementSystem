using AutoMapper;
using LMS.Application.Implementations;
using LMS.Application.Interfaces;
using LMS.Application.ResponseDataModels;
using LMS.Infrastructure.Common.DataModels;
using LMS.Infrastructure.Repositories;
using LMS.Infrastructure.SqlClietInterfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static LMS.Infrastructure.Common.Generic.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LMS.Test.Application.Implementations
{
    public class EnrollmentsManagerTests
    {
        #region Private Fields

        private EnrollmentsManager manager;
        private Mock<IGenericRepository<User>> mockUserRepository;
        private Mock<IGenericRepository<Course>> mockCourseRepository;
        private Mock<IGenericRepository<LearningPath>> mockLearningPathRepository;
        private Mock<IGenericRepository<Enrollment>> mockEnrollmentRepository;
        private Mock<IMapper> mockMapper;
        private Mock<ILogger<IEnrollmentsManager>> mockLogger;

        #endregion

        [SetUp]
        public void Setup()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockUserRepository = new Mock<IGenericRepository<User>>();
            mockCourseRepository = new Mock<IGenericRepository<Course>>();
            mockLearningPathRepository = new Mock<IGenericRepository<LearningPath>>();
            mockEnrollmentRepository = new Mock<IGenericRepository<Enrollment>>();
            mockMapper = new Mock<IMapper>();
            mockLogger = new Mock<ILogger<IEnrollmentsManager>>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IGenericRepository<User>))).Returns(mockUserRepository.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(IGenericRepository<Course>))).Returns(mockCourseRepository.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(IGenericRepository<LearningPath>))).Returns(mockLearningPathRepository.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(IGenericRepository<Enrollment>))).Returns(mockEnrollmentRepository.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(IMapper))).Returns(mockMapper.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<IEnrollmentsManager>))).Returns(mockLogger.Object);

            manager = new EnrollmentsManager(mockServiceProvider.Object);
        }

        #region EnrollCourseAsync

        [Test]
        public async Task GetAllEnrollmentsAsync_WhenCalled_Returns_SSuccessResponse()
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

            var successResponse = new SuccessResponse<IEnumerable<BaseUserProfileViewModel>>
            {
                StatusCode = HttpStatusCode.OK
            };
            var enrollments = new List<Enrollment>
            {
                new Enrollment{ Id = 1}
            };
            var enrollmentView = new EnrollmentViewModel
            {
                Id = 1
            };
            var enrollment = new Enrollment
            {
                Id = 1
            };
            var course = new Course
            {
                Id = 1,
                Title = " ",

            };
            var learningPath = new LearningPath
            {
                Id= 1,
                Title = " ",
            };
        
            int courseId = 1;
            int learningId = 1;
            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == claimDataModel.Email))
                .ReturnsAsync(loggedInUserEntity);
            mockEnrollmentRepository.Setup(repo => repo.FindAllAsync(null))
                .ReturnsAsync(enrollments);
            mockCourseRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Course, bool>>>()))
                .ReturnsAsync(course);
            mockLearningPathRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<LearningPath, bool>>>()))
                .ReturnsAsync(learningPath);
            mockUserRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(loggedInUserEntity);
            mockMapper.Setup(x => x.Map(enrollment, enrollmentView))
                .Returns(new EnrollmentViewModel { Id = 1 });

            // Act
            var response = await manager.GetAllEnrollmentsAsync(claimDataModel, learningId, courseId);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
        }


        [Test]
        public async Task GetAllEnrollmentsAsync_WhenCalled_Returns__Unauthorized_Response()
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

            var errorResponse = new ErrorResponse<string>
            {
                StatusCode = HttpStatusCode.Unauthorized
            };
            var enrollments = new List<Enrollment>
            {
                new Enrollment{ Id = 1}
            };
            var course = new Course
            {
                Id = 1,
                Title = " ",

            };
            var learningPath = new LearningPath
            {
                Id = 1,
                Title = " ",
            };

            int courseId = 1;
            int learningId = 1;
            

            // Act
            var response = await manager.GetAllEnrollmentsAsync(claimDataModel, learningId, courseId);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(errorResponse.StatusCode));
        }


        [Test]
        public async Task GetAllEnrollmentsAsync_WhenCalled_Returns__ForbiddenResponse()
        {
            // Arrange
            var claimDataModel = new UserClaimDataModel { Email = "test@example.com" };
            var loggedInUserEntity = new User
            {
                Email = claimDataModel.Email,
                Name = claimDataModel.Name,
                Role = UserRole.GuestUser.ToString(),
                Id = 1
            };

            var errorResponse = new ErrorResponse<string>
            {
                StatusCode = HttpStatusCode.Forbidden
            };
            var enrollments = new List<Enrollment>
            {
                new Enrollment{ Id = 1}
            };
            var course = new Course
            {
                Id = 1,
                Title = " ",

            };
            var learningPath = new LearningPath
            {
                Id = 1,
                Title = " ",
            };

            int courseId = 1;
            int learningId = 1;
            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == claimDataModel.Email))
                .ReturnsAsync(loggedInUserEntity);

            // Act
            var response = await manager.GetAllEnrollmentsAsync(claimDataModel, learningId, courseId);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(errorResponse.StatusCode));
        }

        #endregion GetAllEnrollmentsAsync

        #region EnrollCourseAsync

        [Test]
        public async Task EnrollCourseAsync_WhenCalled_Returns_SuccessResponse()
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

            var successResponse = new SuccessResponse<IEnumerable<BaseUserProfileViewModel>>
            {
                StatusCode = HttpStatusCode.OK
            };

            var enrollments = new List<Enrollment>
            {
                new Enrollment{ Id = 1}
            };

            var enrollment = new Enrollment
            {
                Id = 1,
            };
            var learningPath = new LearningPath
            {
                Id = 1,
                Title = " ",
            };

            int courseId = 1;
            int learningId = 1;
            int studentId = 1;

            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == claimDataModel.Email))
                .ReturnsAsync(loggedInUserEntity);
            mockEnrollmentRepository.Setup(repo => repo.FindAsync(u => u.CourseId == courseId && u.LearningPathId == learningId && u.StudentId == loggedInUserEntity.Id))
                .ReturnsAsync((Enrollment)null);
            mockEnrollmentRepository.Setup(repo => repo.AddAsync(It.IsAny<Enrollment>()))
                .ReturnsAsync(enrollment);
            mockCourseRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var response = await manager.EnrollCourseAsync(learningId, courseId, claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
        }

        [Test]
        public async Task EnrollCourseAsync_WhenCalled_Returns_Unauthorized_Response()
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

            var errorResponse = new ErrorResponse<string>
            {
                StatusCode = HttpStatusCode.Unauthorized
            };
            var enrollments = new List<Enrollment>
            {
                new Enrollment{ Id = 1}
            };
            var course = new Course
            {
                Id = 1,
                Title = " ",

            };
            var learningPath = new LearningPath
            {
                Id = 1,
                Title = " ",
            };

            int courseId = 1;
            int learningId = 1;

            // Act
            var response = await manager.EnrollCourseAsync(learningId, courseId, claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(errorResponse.StatusCode));
        }

        [Test]
        public async Task EnrollCourseAsync_WhenCalled_Returns__Forbidden_Response()
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

            var successResponse = new ErrorResponse<string>
            {
                StatusCode = HttpStatusCode.Forbidden
            };
            var enrollments = new List<Enrollment>
            {
                new Enrollment{ Id = 1}
            };
            var course = new Course
            {
                Id = 1,
                Title = " ",

            };
            var learningPath = new LearningPath
            {
                Id = 1,
                Title = " ",
            };

            int courseId = 1;
            int learningId = 1;
            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == claimDataModel.Email))
                .ReturnsAsync(loggedInUserEntity);

            // Act
            var response = await manager.EnrollCourseAsync(learningId, courseId, claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
        }

        [Test]
        public async Task EnrollCourseAsync_WhenCalled_Returns_InternalServerErrorResponse()
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

            var successResponse = new ErrorResponse<string>
            { 
                StatusCode = HttpStatusCode.InternalServerError
            };
            var enrollments = new List<Enrollment>
            {
                new Enrollment{ Id = 1}
            };
            var course = new Course
            {
                Id = 1,
                Title = " ",

            };
            var learningPath = new LearningPath
            {
                Id = 1,
                Title = " ",
            };
            var enrollment = new Enrollment
            {
                Id = 1,
            };
            int courseId = 1;
            int learningId = 1;
            mockUserRepository.Setup(repo => repo.FindAsync(u => u.Email == claimDataModel.Email))
                .ReturnsAsync(loggedInUserEntity);
            mockEnrollmentRepository.Setup(repo => repo.FindAsync(u => u.CourseId == courseId && u.LearningPathId == learningId && u.StudentId == loggedInUserEntity.Id))
                .ReturnsAsync((Enrollment)null);

            // Act
            var response = await manager.EnrollCourseAsync(learningId, courseId, claimDataModel);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(successResponse.StatusCode));
        }

        #endregion EnrollCourseAsync

    }
}
