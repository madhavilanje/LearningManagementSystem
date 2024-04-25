using LMS.Api.Controllers;
using LMS.Application.Interfaces;
using LMS.Application.RequestDataModels.Course;
using LMS.Application.RequestDataModels.LearningPath;
using LMS.Application.RequestDataModels.User;
using LMS.Infrastructure.Common.DataModels;
using LMS.Infrastructure.Common.Interfaces;
using LMS.UserManagement.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Test.Api.Controllers
{
    public class LearningPathsControllerTests
    {

        #region Private Fields

        private LearningPathsController learningPathsController;
        private Mock<ILearningPathsManager> mockManager;
        private Mock<IJwtTokenService> mockTokenService;
        private Mock<ILogger<LearningPathsController>> mockLogger;

        #endregion Private Fields

        [SetUp]
        public void Setup()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockManager = new Mock<ILearningPathsManager>();
            mockTokenService = new Mock<IJwtTokenService>();
            mockLogger = new Mock<ILogger<LearningPathsController>>();
            mockServiceProvider.Setup(x => x.GetService(typeof(ILearningPathsManager))).Returns(this.mockManager.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(IJwtTokenService))).Returns(this.mockTokenService.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<LearningPathsController>))).Returns(this.mockLogger.Object);

            learningPathsController = new LearningPathsController(mockServiceProvider.Object);
        }

        #region GetAllLearningPaths

        [Test]
        public async Task GetAllLearningPaths_WhenCalled_Returns_OKResult()
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
            mockManager.Setup(m => m.GetAllLearningPathsAsync())
                .ReturnsAsync(apiResponse);

            // Act
            var result = await learningPathsController.GetAllLearningPaths();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(200));
            var okResult = (ObjectResult)result.Result;
            Assert.That(okResult.Value, Is.SameAs(apiResponse));
        }

        #endregion GetAllLearningPaths

        #region AddLearningPath

        [Test]
        public async Task AddLearningPath_WhenCalled_Returns_OKResult()
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
            LearningPathRequest learningPathRequest = new LearningPathRequest
            {
                Title = "title",
                Description = "description"
            };
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);
            mockManager.Setup(m => m.AddLearningPathAsync(learningPathRequest, claimDataModel))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await learningPathsController.AddLearningPath(learningPathRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(200));
            var okResult = (ObjectResult)result.Result;
            Assert.That(okResult.Value, Is.SameAs(apiResponse));
        }

       /* [Test]
        public async Task AddLearningPath_WhenCalled_Returns_BadRequestResult()
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
            LearningPathRequest learningPathRequest = new LearningPathRequest
            {
                Title = "title",
                Description = "description"
            };
            learningPathsController.ModelState.AddModelError("Error", "Invalid request");
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);

            // Act
            var result = await learningPathsController.AddLearningPath(learningPathRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result.Result);
            Assert.That((result.Result as StatusCodeResult).StatusCode, Is.EqualTo(400));
        }

        #endregion AddLearningPath

        #region RemoveLearningPath

        [Test]
        public async Task RemoveLearningPath_WhenCalled_Returns_OKResult()
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
            RemoveLearningPathRequest removeLearningPathRequest = new RemoveLearningPathRequest
            {
                Title = "title"
            };
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);
            mockManager.Setup(m => m.RemoveLearningPathAsync(removeLearningPathRequest, claimDataModel))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await learningPathsController.RemoveLearningPath(removeLearningPathRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(200));
            var okResult = (ObjectResult)result.Result;
            Assert.That(okResult.Value, Is.SameAs(apiResponse));
        }*/

        /*[Test]
        public async Task RemoveLearningPath_WhenCalled_Returns_BadRequestResult()
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
            RemoveLearningPathRequest removeLearningPathRequest = new RemoveLearningPathRequest
            {
                Title = "title"
            };
            learningPathsController.ModelState.AddModelError("Error", "Invalid request");
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);

            // Act
            var result = await learningPathsController.RemoveLearningPath(removeLearningPathRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result.Result);
            Assert.That((result.Result as StatusCodeResult).StatusCode, Is.EqualTo(400));
        }*/

        #endregion RemoveCourse

        #region GetAllCourses

        [Test]
        public async Task GetAllCourses_WhenCalled_Returns_OKResult()
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
            int learningPathId = 0;
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);
            mockManager.Setup(m => m.GetAllCoursesAsync(learningPathId))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await learningPathsController.GetAllCourses(learningPathId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(200));
            var okResult = (ObjectResult)result.Result;
            Assert.That(okResult.Value, Is.SameAs(apiResponse));
        }

        #endregion GetAllLearningPaths   

        #region AddCourseAsync

        [Test]
        public async Task AddCourseAsync_WhenCalled_Returns_OKResult()
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
            CourseRequest courseRequest = new CourseRequest
            {
                Title = "title",
                ContentUrl = "",
                Description = " "
            };
            int learningPathId = 0;
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);
            mockManager.Setup(m => m.AddCourseAsync(courseRequest, claimDataModel, learningPathId))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await learningPathsController.AddCourseAsync(courseRequest, learningPathId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(200));
            var okResult = (ObjectResult)result.Result;
            Assert.That(okResult.Value, Is.SameAs(apiResponse));
        }

        [Test]
        public async Task AddCourseAsync_WhenCalled_Returns_BadRequestResult()
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
            CourseRequest courseRequest = new CourseRequest
            {
                Title = "title",
                ContentUrl = "",
                Description = " "
            };
            int learningPathId = 0;
            learningPathsController.ModelState.AddModelError("Error", "Invalid request");
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);

            // Act
            var result = await learningPathsController.AddCourseAsync(courseRequest, learningPathId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result.Result);
            Assert.That((result.Result as StatusCodeResult).StatusCode, Is.EqualTo(400));
        }

        #endregion RemoveCourse

        #region RemoveCourse
/*
        [Test]
        public async Task RemoveCourse_WhenCalled_Returns_OKResult()
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
            RemoveCourseRequest removeCourseRequest = new RemoveCourseRequest
            {
                Title = "title"
            };
            int id = 0;
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);
            mockManager.Setup(m => m.RemoveCourseAsync(, claimDataModel, id))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await learningPathsController.RemoveCourse(removeCourseRequest, id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(200));
            var okResult = (ObjectResult)result.Result;
            Assert.That(okResult.Value, Is.SameAs(apiResponse));
        }

        [Test]
        public async Task RemoveCourse_WhenCalled_Returns_NoContentResult()
        {
            //Arrange
            var apiResponse = new ApiResponse
            {
                StatusCode = HttpStatusCode.NoContent
            };
            var claimDataModel = new UserClaimDataModel
            {
                Email = "sampleuser@hitachivantara.com"
            };
            RemoveCourseRequest removeCourseRequest = new RemoveCourseRequest
            {
                Title = "title"
            };
            int id = 0;
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);
            mockManager.Setup(m => m.RemoveCourseAsync(removeCourseRequest, claimDataModel, id))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await learningPathsController.RemoveCourse(removeCourseRequest, id);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task RemoveCourse_WhenCalled_Returns_BadRequestResult()
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
            RemoveCourseRequest removeCourseRequest = new RemoveCourseRequest
            {
                Title = "title"
            };
            int id = 0;
            learningPathsController.ModelState.AddModelError("Error", "Invalid request");
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);

            // Act
            var result = await learningPathsController.RemoveCourse(removeCourseRequest, id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result.Result);
            Assert.That((result.Result as StatusCodeResult).StatusCode, Is.EqualTo(400));
        }*/

        #endregion RemoveCourse

    }
}