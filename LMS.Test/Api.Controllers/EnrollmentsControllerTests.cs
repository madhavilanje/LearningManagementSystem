using LMS.Api.Controllers;
using LMS.Application.Interfaces;
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
    public class EnrollmentsControllerTests
    {
        #region Private Fields

        private EnrollmentsController enrollmentsController;
        private Mock<IEnrollmentsManager> mockEnrollmentsManager;
        private Mock<IJwtTokenService> mockTokenService;
        private Mock<ILogger<EnrollmentsController>> mockLogger;

        #endregion Private Fields

        [SetUp]
        public void Setup()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockEnrollmentsManager = new Mock<IEnrollmentsManager>();
            mockTokenService = new Mock<IJwtTokenService>();
            mockLogger = new Mock<ILogger<EnrollmentsController>>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IEnrollmentsManager))).Returns(this.mockEnrollmentsManager.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(IJwtTokenService))).Returns(this.mockTokenService.Object);
            mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<EnrollmentsController>))).Returns(this.mockLogger.Object);

            enrollmentsController = new EnrollmentsController(mockServiceProvider.Object);
        }

        #region GetAllEnrollments

        [Test]
        public async Task GetAllEnrollments_WhenCalled_Returns_OKResult()
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
            int learningPathId = 1;
            int courseId = 1;
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);
            mockEnrollmentsManager.Setup(m => m.GetAllEnrollmentsAsync(It.IsAny<UserClaimDataModel>(), learningPathId, courseId))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await enrollmentsController.GetAllEnrollments(learningPathId, courseId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(200));
            var okResult = (ObjectResult)result.Result;
            Assert.That(okResult.Value, Is.SameAs(apiResponse));
        }

        #endregion GetAllEnrollments

        #region EnrollCourse

        [Test]
        public async Task EnrollCourse_WhenCalled_Returns_OKResult()
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
            int learningPathId = 1;
            int courseId = 1;
            mockTokenService.Setup(t => t.GetClaimData(It.IsAny<HttpContext>()))
                .Returns(claimDataModel);
            mockEnrollmentsManager.Setup(m => m.EnrollCourseAsync(learningPathId, courseId, claimDataModel))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await enrollmentsController.EnrollCourse(learningPathId, courseId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(200));
            var okResult = (ObjectResult)result.Result;
            Assert.That(okResult.Value, Is.SameAs(apiResponse));
        }

        #endregion EnrollCourse
    }

   }
