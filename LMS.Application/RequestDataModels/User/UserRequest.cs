using System.ComponentModel.DataAnnotations;
using LMS.Application.CustomAttribute;
using System.Text.Json.Serialization;

namespace LMS.Application.RequestDataModels.User
{
    public class UserRequest
    {
        [Required]
        [RegularExpression(@"^[A-Za-z]+(?:\s[A-Za-z]+)*$", ErrorMessage = "Enter only letters.")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [EmailAddressValidation(ErrorMessage = "Email address is invalid.")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9\s]).+$", ErrorMessage = "Enter at least 1 capital letter, number and special character is required.")]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [Required]
        [RoleValidation(ErrorMessage = "Role is invalid")]
        [JsonPropertyName("role")]
        public string Role { get; set; }

    }
}
