using System;
using System.Collections.Generic;
using System.Text;

namespace Isabella.Common.Dtos.Users
{
    public class GetRegisterUserModeFastDto
    {
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string PasswordGuid { get; set; }
    }
}
