using Microsoft.AspNetCore.Mvc;
using LMS.Infrastructure.Common.DataModels;
using LMS.Infrastructure.Common.Interfaces;
using LMS.Application.Interfaces;
using LMS.Application.RequestDataModels.User;
using Microsoft.AspNetCore.Authorization;
using LMS.Application.ResponseDataModels;
using LMS.Infrastructure.Common.Generic;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;



namespace LMS.UserManagement.Api.Controllers
{
    [ApiController]
    public class UsersController : RootController
    {

        #region Private Fields

        private readonly IUsersManager manager;
        private readonly IJwtTokenService tokenService;
        private readonly ILogger<UsersController> logger;

        #endregion Private Fields


        public UsersController(IServiceProvider serviceProvider)
        {
            this.manager = serviceProvider.GetRequiredService<IUsersManager>();
            this.tokenService = serviceProvider.GetRequiredService<IJwtTokenService>();
            this.logger = serviceProvider.GetRequiredService<ILogger<UsersController>>();

        }

        #region HttpGet

        /// <summary>
        /// This method is used to get logged in user profile.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("me/profile")]
        public async Task<ActionResult<ApiResponse>> GetLoggedInUserProfileAsync()
        {
            UserClaimDataModel claimDataModel = tokenService.GetClaimData(HttpContext);
            logger.LogInformation("UsersController/GetLoggedInUserProfileAsync - User {Email} requested to get user profile.", claimDataModel.Email);

            if (!ModelState.IsValid) { return BadRequest(); }

            var apiResponse = await manager.GetLoggedInUserProfileAsync(claimDataModel);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        /// <summary>
        /// This method is used to get logged in user profile.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("discussions")]
        public async Task<ActionResult<ApiResponse>> GetMessageAsync()
        {
            UserClaimDataModel claimDataModel = tokenService.GetClaimData(HttpContext);

            var apiResponse = await manager.GetMessageAsync(claimDataModel);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        /// <summary>
        /// This method is used to get all users
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAllUsersAsync()
        {
            UserClaimDataModel claimDataModel = tokenService.GetClaimData(HttpContext);
            var apiResponse = await manager.GetAllUsersAsync(claimDataModel);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        /// <summary>
        /// This API is used to get all enrolled student into the course
        /// </summary>
        /// <param name="userClaimData"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("{studentId}/enrollments")]
        public async Task<ActionResult<ApiResponse>> GetEnrollmentsByStudentIdAsync(int studentId)
        {
            UserClaimDataModel claimDataModel = tokenService.GetClaimData(HttpContext);

            var apiResponse = await manager.GetEnrollmentsBasedOnStudentIdAsync(claimDataModel, studentId);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        #endregion HttpGet

        #region HttpPost

        /// <summary>
        /// This API is used to login.
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<ApiResponse>> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid) { return BadRequest(); }

            var apiResponse = await manager.LoginAsync(loginRequest);

            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        /// <summary>
        /// This API is used to add new user.
        /// </summary>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<ApiResponse>> RegisterUserAsync([FromBody] UserRequest userRequest)
        {
            if (!ModelState.IsValid) { return BadRequest(); }

            var apiResponse = await manager.RegisterUser(userRequest);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        /// <summary>
        /// This API is used to add new message in forum.
        /// </summary>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("discussions/send")]
        public async Task<ActionResult<ApiResponse>> SendMessageAsync([FromBody] MessageRequest request)
        {
            UserClaimDataModel claimDataModel = tokenService.GetClaimData(HttpContext);

            if (!ModelState.IsValid) { return BadRequest(); }

            var apiResponse = await manager.SendMessageAsync(request, claimDataModel);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        #endregion

        #region HttpPut

        /// <summary>
        /// This API is used to change usersrole.
        /// </summary>
        /// <param name="assignRoleRequest"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("assign-role")]
        public async Task<ActionResult<ApiResponse>> AssignRoleAsync([FromBody] AssignRoleRequest assignRoleRequest)
        {
            UserClaimDataModel claimDataModel = tokenService.GetClaimData(HttpContext);
            if (!ModelState.IsValid) { return BadRequest(); }

            var apiResponse = await manager.AssignRoleAsync(assignRoleRequest, claimDataModel);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        #endregion

        #region HttpDelete

        /// <summary>
        /// This API is used to remove a user.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("remove")]
        public async Task<ActionResult<ApiResponse>> RemoveUserAsync([FromBody] RemoveUserRequest removeUserRequest)
        {
            UserClaimDataModel claimDataModel = tokenService.GetClaimData(HttpContext);

            if (!ModelState.IsValid) { return BadRequest(); }
            var apiResponse = await manager.RemoveUserAsync(removeUserRequest, claimDataModel);
            return CheckStatusCodeAndGetResponse(apiResponse, logger);
        }

        #endregion


    }
}