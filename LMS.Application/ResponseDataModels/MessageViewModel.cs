using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Application.ResponseDataModels
{
    public class MessageViewModel
    {

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; }
    }
}
