using System;
using System.Collections.Generic;
using System.Text;

namespace HondaERP.Infrastructure.Identity
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public string Key { get; set; } = "";
        public int ExpiryMinutes { get; set; } = 30;
    }
}
