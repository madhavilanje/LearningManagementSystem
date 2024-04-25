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
    public class RemoveUserRequest
    {

        [Required]
        [EmailAddressValidation(ErrorMessage = "Email address in invalid.")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

    }
}
