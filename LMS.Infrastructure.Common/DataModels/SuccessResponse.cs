using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Common.DataModels
{
    public class SuccessResponse<T> : ApiResponse
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
