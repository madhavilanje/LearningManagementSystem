using LMS.Application.Interfaces;
using  LMS.Application.RequestDataModels;
using LMS.Application.RequestDataModels.User;
using LMS.Infrastructure.Common.DataModels;
using LMS.Infrastructure.Common.Interfaces;
using LMS.UserManagement.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Net;
namespace LMS.Test.Api.Controllers
{
    public class UsersControllerTests
    {
        #region Private Fields

        private UsersController usersController;
        private Mock<IUsersManager> mockUserManager;
        private Mock<IJwtTokenService> mockTokenService;
        private Mock<ILogger<UsersController>> mockLogger;

        #endregion

        [SetUp]
        public void Setup()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockUserManager = new Mock<IUsersManager>();
            mockTokenService = new Mock<IJwtTokenService>();
            mockLogger = new Mock<ILogger<UsersController>>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IUsersManager))).Returns(this.mockUserManager.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(IJwtTokenService))).Returns(this.mockTokenService.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<UsersController>))).Returns(this.mockLogger.Object);

            usersController = new UsersController(mockServiceProvider.Object);
        }

        #region GetEnrollmentsByStudentId

        [Test]
        public async Task GetEnrollmentsByStudentId_WhenCalled_Returns_OKResult()
        {
            //Arrange
            var apiResponse = new ApiResponse
            {
                StatusCode = HttpStatusCode.OK
            };
            var claimDataModel = new UserClaimDataModel
            {
                Email = "sampleuser@hitachivantara.com"
            };
            int studentId = 1;
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);
            mockUserManager.Setup(m => m.GetEnrollmentsBasedOnStudentIdAsync(It.IsAny<UserClaimDataModel>(), studentId))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await usersController.GetEnrollmentsByStudentIdAsync(studentId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(200));
            var okResult = (ObjectResult)result.Result;
            Assert.That(okResult.Value, Is.SameAs(apiResponse));
        }

        #endregion GetEnrollmentsByStudentId

        #region GetAllUsersAsyn

        [Test]
        public async Task GetAllUsersAsync_WhenCalled_Returns_OKResult()
        {
            //Arrange
            var apiResponse = new ApiResponse
            {
                StatusCode = HttpStatusCode.OK
            };
            var claimDataModel = new UserClaimDataModel
            {
                Email = "sampleuser@hitachivantara.com"
            };
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);
            mockUserManager.Setup(m => m.GetAllUsersAsync(It.IsAny<UserClaimDataModel>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await usersController.GetAllUsersAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(200));
            var okResult = (ObjectResult)result.Result;
            Assert.That(okResult.Value, Is.SameAs(apiResponse));
        }

        #endregion GetAllUsersAsyn

        #region LoginAsync

        [Test]
        public async Task LoginAsync_WhenCalled_Returns_OKResult()
        {
            //Arrange
            var loginRequest = new LoginRequest
            {
                Email = " ",
                Password= ""
            };
            var apiResponse = new ApiResponse
            {
                StatusCode = HttpStatusCode.OK
            };
            var claimDataModel = new UserClaimDataModel
            {
                Email = "sampleuser@hitachivantara.com"
            };
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);
            mockUserManager.Setup(m => m.LoginAsync(loginRequest))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await usersController.LoginAsync(loginRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(200));
            var okResult = (ObjectResult)result.Result;
            Assert.That(okResult.Value, Is.SameAs(apiResponse));
        }

        [Test]
        public async Task LoginAsync_WhenCalled_Returns_BadRequestResult()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = " "
            };
            var claimDataModel = new UserClaimDataModel
            {
                Email = "sampleuser2@hitachivantara.com"
            };
            usersController.ModelState.AddModelError("Error", "Invalid login request");
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);

            // Act
            var result = await usersController.LoginAsync(loginRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result.Result);
            Assert.That((result.Result as StatusCodeResult).StatusCode, Is.EqualTo(400));

        }

        #endregion LoginAsync

        #region RegisterUserAsync

        [Test]
        public async Task RegisterUserAsync_WhenCalled_Returns_OKResult()
        {
            //Arrange
            var userRequest = new UserRequest
            {
                Email = " "
            };
            var apiResponse = new ApiResponse
            {
                StatusCode = HttpStatusCode.OK
            };
            var claimDataModel = new UserClaimDataModel
            {
                Email = "sampleuser@hitachivantara.com"
            };
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);
            mockUserManager.Setup(m => m.RegisterUser(userRequest))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await usersController.RegisterUserAsync(userRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(200));
            var okResult = (ObjectResult)result.Result;
            Assert.That(okResult.Value, Is.SameAs(apiResponse));
        }

        [Test]
        public async Task RegisterUserAsync_WhenCalled_Returns_BadRequestResult()
        {
            // Arrange
            var userRequest = new UserRequest
            {
                Email = " "
            };
            var claimDataModel = new UserClaimDataModel
            {
                Email = "sampleuser2@hitachivantara.com"
            };
            usersController.ModelState.AddModelError("Error", "Invalid user request");
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);

            // Act
            var result = await usersController.RegisterUserAsync(userRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result.Result);
            Assert.That((result.Result as StatusCodeResult).StatusCode, Is.EqualTo(400));

        }

        #endregion RegisterUserAsync

        #region AssignRoleAsync

        [Test]
        public async Task AssignRoleAsync_WhenCalled_Returns_OKResult()
        {
            //Arrange
            var assignRoleRequest = new AssignRoleRequest
            {
                Email = " "
            };
            var apiResponse = new ApiResponse
            {
                StatusCode = HttpStatusCode.OK
            };
            var claimDataModel = new UserClaimDataModel
            {
                Email = "sampleuser@hitachivantara.com"
            };
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);
            mockUserManager.Setup(m => m.AssignRoleAsync(assignRoleRequest, It.IsAny<UserClaimDataModel>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await usersController.AssignRoleAsync(assignRoleRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(200));
            var okResult = (ObjectResult)result.Result;
            Assert.That(okResult.Value, Is.SameAs(apiResponse));
        }

        [Test]
        public async Task AssignRoleAsync_WhenCalled_Returns_BadRequestResult()
        {
            // Arrange
            var assignRoleRequest = new AssignRoleRequest
            {
                Email = " "
            };
            var claimDataModel = new UserClaimDataModel
            {
                Email = "sampleuser2@hitachivantara.com"
            };
            usersController.ModelState.AddModelError("Error", "Invalid user request");
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);

            // Act
            var result = await usersController.AssignRoleAsync(assignRoleRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result.Result);
            Assert.That((result.Result as StatusCodeResult).StatusCode, Is.EqualTo(400));

        }

        #endregion AssignRoleAsync

        #region RemoveUserAsync

        [Test]
        public async Task RemoveUserAsync_WhenCalled_Returns_OKResult()
        {
            //Arrange
            var removeUserRequest = new RemoveUserRequest
            {
                Email = " "
            };
            var apiResponse = new ApiResponse
            {
                StatusCode = HttpStatusCode.OK
            };
            var claimDataModel = new UserClaimDataModel
            {
                Email = "sampleuser@hitachivantara.com"
            };
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);
            mockUserManager.Setup(m => m.RemoveUserAsync(removeUserRequest, It.IsAny<UserClaimDataModel>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await usersController.RemoveUserAsync(removeUserRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(200));
            var okResult = (ObjectResult)result.Result;
            Assert.That(okResult.Value, Is.SameAs(apiResponse));
        }

        [Test]
        public async Task RemoveUserAsync_WhenCalled_Returns_BadRequestResult()
        {
            // Arrange
            var removeUserRequest = new RemoveUserRequest
            {
                Email = " "
            };
            var claimDataModel = new UserClaimDataModel
            {
                Email = "sampleuser2@hitachivantara.com"
            };
            usersController.ModelState.AddModelError("Error", "Invalid user request");
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);

            // Act
            var result = await usersController.RemoveUserAsync(removeUserRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result.Result);
            Assert.That((result.Result as StatusCodeResult).StatusCode, Is.EqualTo(400));

        }

        #endregion RegisterUserAsync

    }
}