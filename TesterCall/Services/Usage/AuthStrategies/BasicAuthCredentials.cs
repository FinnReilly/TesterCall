using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Services.Usage.AuthStrategies.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.Usage.AuthStrategies
{
    public class BasicAuthCredentials : IGetAuthorisationHeaderStrategy
    {
        private readonly IBase64EncodeService _base64Converter;
        private string _userName;
        private string _password;
       
        public BasicAuthCredentials(IBase64EncodeService base64EncodeService,
                                    string userName,
                                    string password)
        {
            _base64Converter = base64EncodeService;
            _userName = userName;
            _password = password;
        }

        public string UserName => _userName;
        public string Password => _password;

        public Task<string> GetHeader()
        {
            var encodedCreds = _base64Converter.Encode(_userName,
                                                        _password);

            return Task.FromResult($"Basic {encodedCreds}");
        }
    }
}
