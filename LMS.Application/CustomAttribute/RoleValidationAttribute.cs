using System.ComponentModel.DataAnnotations;
using static LMS.Infrastructure.Common.Generic.Enums;

namespace LMS.Application.CustomAttribute
{
    public class RoleValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string role = value as string;

            if (string.IsNullOrEmpty(role))
                return false;
            
            return role.Equals(UserRole.Instructor.ToString(), StringComparison.OrdinalIgnoreCase) 
                || role.Equals(UserRole.Student.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
