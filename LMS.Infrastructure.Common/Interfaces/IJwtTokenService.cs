using Microsoft.AspNetCore.Http;
using LMS.Infrastructure.Common.DataModels;

namespace LMS.Infrastructure.Common.Interfaces
{
    public interface IJwtTokenService
    {
        UserClaimDataModel GetClaimData(HttpContext httpContext);

    }
}
