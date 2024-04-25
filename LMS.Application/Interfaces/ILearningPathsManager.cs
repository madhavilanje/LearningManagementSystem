using LMS.Application.RequestDataModels.Course;
using LMS.Application.RequestDataModels.LearningPath;
using LMS.Application.RequestDataModels.User;
using LMS.Infrastructure.Common.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Interfaces
{
    public interface ILearningPathsManager
    {
        #region Get Operation

        /// <summary>
        /// This method is used to get all learning paths.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse> GetAllLearningPathsAsync();

        #endregion Get Operation

        #region Post Operation

        /// <summary>
        /// This method is used to add a learning path.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse> AddLearningPathAsync(LearningPathRequest request, UserClaimDataModel claimDataModel);

        #endregion Post Operation

        #region Delete Operation

        /// <summary>
        /// this method is used to remove a larning path from database
        /// </summary>
        /// <param name="removeLearningPathRequest"></param>
        /// <returns></returns>
        Task<ApiResponse> RemoveLearningPathAsync(UserClaimDataModel claimDataModel, int id);

        #endregion Delete Operation
        #region Courses

        #region Get Operation

        /// <summary>
        /// This method is used to get all courses.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse> GetAllCoursesAsync(int learningPathId);

        #endregion Get Operation

        #region Post Operation

        /// <summary>
        /// This method is used to add a course.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse> AddCourseAsync(CourseRequest courseRequest, UserClaimDataModel claimDataModel, int learningPathId);

        #endregion Post Operation

        #region Delete Operation

        /// <summary>
        /// this method is used to remove a course from database
        /// </summary>
        /// <param name="removeCourseRequest"></param>
        /// <returns></returns>
        Task<ApiResponse> RemoveCourseAsync(UserClaimDataModel claimDataModel, int learningPathId, int courseId);

        #endregion Delete Operation

        #endregion Courses
    }
}
