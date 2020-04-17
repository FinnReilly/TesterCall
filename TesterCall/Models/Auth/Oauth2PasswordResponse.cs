using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Models.Auth
{
    public class Oauth2PasswordResponse : Oauth2BaseResponse
    {
        public string RefreshToken { get; set; }
    }
}
