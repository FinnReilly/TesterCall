using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Models.Auth
{
    public class Oauth2BaseResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}
