using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Common.DataModels
{
    public class UserClaimDataModel
    {
        public string Name { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string JwtToken { get; set; }
    }
}
