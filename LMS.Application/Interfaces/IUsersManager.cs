using LMS.Application.RequestDataModels.User;
using LMS.Infrastructure.Common.DataModels;

namespace LMS.Application.Interfaces
{
    public interface IUsersManager
    {
        #region Get Operation

        /// <summary>
        /// This method is used to get logged in user profile.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse> GetAllUsersAsync(UserClaimDataModel claimDataModel);

        /// <summary>
        /// This method is used to get all messaged
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse> GetMessageAsync(UserClaimDataModel claimDataModel);

        /// <summary>
        /// This method is used to get logged in user profile.
        /// </summary>
        /// <param name="claimDataModel"></param>
        /// <returns></returns>
        Task<ApiResponse> GetLoggedInUserProfileAsync(UserClaimDataModel claimDataModel);


        /// <summary>
        /// This API is used to get all enrolled courses by a student
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse> GetEnrollmentsBasedOnStudentIdAsync(UserClaimDataModel userClaimData, int studentId);

        #endregion Get Operation

        #region Post Operation

        /// <summary>
        /// This method is used to login into the portal.
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        Task<ApiResponse> LoginAsync(LoginRequest loginRequest);

        /// <summary>
        /// This method is used to send message in discussion forum
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse> SendMessageAsync(MessageRequest loginRequest, UserClaimDataModel claimDataModel);


        /// <summary>
        /// This method is used to register new user
        /// </summary>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        Task<ApiResponse> RegisterUser(UserRequest userRequest);

        #endregion post Operation

        #region Put Operation

        /// <summary>
        /// This method is used to assign user new role
        /// </summary>
        /// <param name="assignRoleRequest"></param>
        /// <returns></returns>
        Task<ApiResponse> AssignRoleAsync(AssignRoleRequest assignRoleRequest, UserClaimDataModel claimDataModel);

        #endregion Put Operation


        #region Delete Operation

        /// <summary>
        /// this method is used to remove a user from database
        /// </summary>
        /// <param name="removeUserRequest"></param>
        /// <returns></returns>
        Task<ApiResponse> RemoveUserAsync(RemoveUserRequest removeUserRequest, UserClaimDataModel claimDataModel);


        #endregion Delete Operation



    }
}
