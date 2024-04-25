using LMS.Application.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Application.RequestDataModels.User
{
    public class MessageRequest
    {
        [Required]
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
