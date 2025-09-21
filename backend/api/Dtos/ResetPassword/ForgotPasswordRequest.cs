using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.ResetPassword
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = default!;
    }
}