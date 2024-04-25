using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using LMS.Infrastructure.Common.DataModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.UserManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RootController : ControllerBase
    {
        protected ActionResult CheckStatusCodeAndGetResponse<T>(ApiResponse responseData, ILogger<T> logger)
        {
            logger.LogInformation("RootController : CheckStatusCodeAndGetResponse: responseData - {0}", JsonConvert.SerializeObject(responseData));

            if (responseData.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return new StatusCodeResult(Convert.ToInt32(responseData.StatusCode));
            }
            return new ObjectResult(responseData)
            {
                StatusCode = Convert.ToInt32(responseData.StatusCode)
            };
        }
    } 
}
