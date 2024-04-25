using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Common.Generic
{
    public class AppSettings
    {
        public Jwt jwt { get; set; }

    }
    public class Jwt
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }

        public string Key { get; set; }


    }
}
