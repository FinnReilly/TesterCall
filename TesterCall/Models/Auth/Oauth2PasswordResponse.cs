using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Models.Auth
{
    public class Oauth2PasswordResponse : Oauth2BaseResponse
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
