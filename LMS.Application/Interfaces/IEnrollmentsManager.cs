using LMS.Application.RequestDataModels.LearningPath;
using LMS.Infrastructure.Common.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Interfaces
{
    public interface IEnrollmentsManager
    {
        #region Get Operation

        /// <summary>
        /// This API is used to get all enrolled students into the course
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse> GetAllEnrollmentsAsync(UserClaimDataModel userClaimData, int? learningPathId, int? courseId);

        
        #endregion Get Operation

        #region Post Operation

        /// <summary>
        /// This API is used to enroll student into the course
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse> EnrollCourseAsync(int learningPathId, int courseId, UserClaimDataModel userClaimData);

        #endregion Post Operation
    }
}
