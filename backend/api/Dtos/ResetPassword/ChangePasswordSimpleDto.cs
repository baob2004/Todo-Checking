using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.ResetPassword
{
    public class ChangePasswordSimpleDto
    {
        public string Username { get; set; } = default!;
        public string OldPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }
}