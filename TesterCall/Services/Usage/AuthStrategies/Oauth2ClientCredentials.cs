using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Models.Auth;
using TesterCall.Services.Usage.AuthStrategies.Interfaces;
using TesterCall.Services.Usage.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.Usage.AuthStrategies
{
    public class Oauth2ClientCredentials : IGetAuthorisationHeaderStrategy
    {
        private readonly IDateTimeWrapper _dateService;
        private readonly IPostUrlFormEncodedService _postUrlEncodedService;

        private string _tokenUri;
        private string _clientId;
        private string _clientSecret;
        private DateTime _expiryTime;
        private Oauth2BaseResponse _lastResponse;

        public Oauth2ClientCredentials(IDateTimeWrapper dateTimeWrapper,
                                        IPostUrlFormEncodedService postUrlFormEncodedService,
                                        string tokenUri,
                                        string clientId,
                                        string clientSecret)
        {
            _dateService = dateTimeWrapper;
            _postUrlEncodedService = postUrlFormEncodedService;
            _tokenUri = tokenUri;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public Oauth2BaseResponse LastResponse => _lastResponse;

        public async Task<string> GetHeader()
        {
            if (_lastResponse == null || _dateService.Now >= _expiryTime)
            {
                _lastResponse = await _postUrlEncodedService
                                        .GetPostResult<Oauth2BaseResponse>(_tokenUri,
                                                                            new Dictionary<string, string>()
                                                                            {
                                                                                { "client_id", _clientId },
                                                                                { "client_secret", _clientSecret },
                                                                                { "grant_type", "client_credentials" }
                                                                            });

                _expiryTime = _dateService.Now.AddSeconds(_lastResponse.ExpiresIn - 5);
            }

            return $"{_lastResponse.TokenType} {_lastResponse.AccessToken}";
        }
    }
}
