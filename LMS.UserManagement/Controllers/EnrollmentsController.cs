using LMS.Application.Interfaces;
using LMS.Application.RequestDataModels.LearningPath;
using LMS.Infrastructure.Common.DataModels;
using LMS.Infrastructure.Common.Interfaces;
using LMS.UserManagement.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers
{
    [ApiController]
    public class EnrollmentsController : RootController
    {
        #region Private Fields

        private readonly IEnrollmentsManager manager;
        private readonly IJwtTokenService tokenService;
        private readonly ILogger<EnrollmentsController> logger;

        #endregion Private Fields


        public EnrollmentsController(IServiceProvider serviceProvider)
        {
            this.manager = serviceProvider.GetRequiredService<IEnrollmentsManager>();
            this.tokenService = serviceProvider.GetRequiredService<IJwtTokenService>();
            this.logger = serviceProvider.GetRequiredService<ILogger<EnrollmentsController>>();

        }

        #region HttpGet

        /// <summary>
        /// This API is used to get all enrolled student into the course
        /// </summary>
        /// <param name="userClaimData"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAllEnrollments(int? learningPathId = null, int? courseId = null)
        {
            UserClaimDataModel claimDataModel = tokenService.GetClaimData(HttpContext);

            var apiResponse = await manager.GetAllEnrollmentsAsync(claimDataModel, learningPathId, courseId);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        

        #endregion HttpGet

        #region HttpPost

        /// <summary>
        /// This API is used to enroll student into the course
        /// </summary>
        /// <param name="userClaimData"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("{learningPathId}/{courseId}/enroll")]
        public async Task<ActionResult<ApiResponse>> EnrollCourse(int learningPathId, int courseId)
        {
            UserClaimDataModel claimDataModel = tokenService.GetClaimData(HttpContext);

            var apiResponse = await manager.EnrollCourseAsync(learningPathId, courseId, claimDataModel);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        #endregion HttpPost
    }
}
