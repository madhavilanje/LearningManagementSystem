using LMS.Application.Interfaces;
using LMS.Application.RequestDataModels.Course;
using LMS.Application.RequestDataModels.User;
using LMS.Infrastructure.Common.DataModels;
using LMS.Infrastructure.Common.Interfaces;
using LMS.Infrastructure.Repositories;
using LMS.UserManagement.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LMS.Infrastructure.Common.Generic.Enums;
using System.Net;
using LMS.Application.RequestDataModels.LearningPath;

namespace LMS.Api.Controllers
{
    [ApiController]
    public class LearningPathsController : RootController
    {
        #region Private Fields

        private readonly ILearningPathsManager manager;
        private readonly IJwtTokenService tokenService;
        private readonly ILogger<LearningPathsController> logger;

        #endregion Private Fields


        public LearningPathsController(IServiceProvider serviceProvider)
        {
            this.manager = serviceProvider.GetRequiredService<ILearningPathsManager>();
            this.tokenService = serviceProvider.GetRequiredService<IJwtTokenService>();
            this.logger = serviceProvider.GetRequiredService<ILogger<LearningPathsController>>();
        }


        #region HttpGet

        /// <summary>
        /// This method is used to get all learning paths
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAllLearningPaths()
        {
            var apiResponse = await manager.GetAllLearningPathsAsync();
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        #endregion HttpGet

        #region HttpPost

        /// <summary>
        /// This API is used to add new learning path.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("add-learning-path")]
        public async Task<ActionResult<ApiResponse>> AddLearningPath([FromBody] LearningPathRequest request)
        {
            UserClaimDataModel claimDataModel = tokenService.GetClaimData(HttpContext);

            if (!ModelState.IsValid) { return BadRequest(); }

            var apiResponse = await manager.AddLearningPathAsync(request, claimDataModel);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        #endregion HttpPost

        #region HttpDelete

        /// <summary>
        /// This API is used to remove a learning path.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<ApiResponse>> RemoveLearningPath(int id)
        {
            UserClaimDataModel claimDataModel = tokenService.GetClaimData(HttpContext);

            if (!ModelState.IsValid) { return BadRequest(); }
            var apiResponse = await manager.RemoveLearningPathAsync(claimDataModel, id);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        #endregion

        #region Courses

        #region HttpGet

        /// <summary>
        /// This method is used to get all courses
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("{learningPathId}")]

        public async Task<ActionResult<ApiResponse>> GetAllCourses(int learningPathId)
        {
            var apiResponse = await manager.GetAllCoursesAsync(learningPathId);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        #endregion HttpGet

        #region HttpPost

        /// <summary>
        /// This API is used to add new course.
        /// </summary>
        /// <param name="courseRequest"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("{learningPathId}/add-course")]
        public async Task<ActionResult<ApiResponse>> AddCourseAsync([FromBody] CourseRequest courseRequest, int learningPathId)
        {
            UserClaimDataModel claimDataModel = tokenService.GetClaimData(HttpContext);

            if (!ModelState.IsValid) { return BadRequest(); }

            var apiResponse = await manager.AddCourseAsync(courseRequest, claimDataModel, learningPathId);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        #endregion

        #region HttpDelete

        /// <summary>
        /// This API is used to remove a course.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{learningPathId}/{courseId}")]
        public async Task<ActionResult<ApiResponse>> RemoveCourse(int learningPathId, int courseId)
        {
            UserClaimDataModel claimDataModel = tokenService.GetClaimData(HttpContext);

            if (!ModelState.IsValid) { return BadRequest(); }
            var apiResponse = await manager.RemoveCourseAsync(claimDataModel, learningPathId, courseId);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        #endregion

        #endregion Courses
    }
}
