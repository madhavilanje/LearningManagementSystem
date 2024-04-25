using Newtonsoft.Json;

namespace LMS.Infrastructure.Common.DataModels
{
    public class ErrorResponse<T> : ApiResponse
    {
        [JsonProperty("error")]
        public T Error { get; set; }
    }
}
