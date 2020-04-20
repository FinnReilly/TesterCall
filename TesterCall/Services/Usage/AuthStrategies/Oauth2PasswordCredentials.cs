using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Models;
using TesterCall.Models.Auth;
using TesterCall.Services.Usage.AuthStrategies.Interfaces;
using TesterCall.Services.Usage.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.Usage.AuthStrategies
{
    public class Oauth2PasswordCredentials : IGetAuthorisationHeaderStrategy, IHasResponseTime
    {
        private readonly IDateTimeWrapper _dateService;
        private readonly IPostUrlFormEncodedService _postUrlEncodedService;

        private string _tokenUri;
        private string _clientId;
        private string _clientSecret;
        private string _userName;
        private string _password;
        private DateTime _expiryTime;
        private TimeSpan _lastResponseTime;
        private Oauth2PasswordResponse _lastResponse;

        public Oauth2PasswordCredentials(IDateTimeWrapper dateTimeWrapper,
                                        IPostUrlFormEncodedService postUrlFormEncodedService,
                                        string tokenUri,
                                        string clientId,
                                        string clientSecret,
                                        string userName,
                                        string password)
        {
            _dateService = dateTimeWrapper;
            _postUrlEncodedService = postUrlFormEncodedService;
            _tokenUri = tokenUri;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _userName = userName;
            _password = password;
        }

        public Oauth2PasswordResponse LastResponse => _lastResponse;
        public string TokenUri => _tokenUri;
        public string ClientId => _clientId;
        public string ClientSecret => _clientSecret;
        public string UserName => _userName;
        public string Password => _password;
        public DateTime ExpectedExpiry => _expiryTime;
        public TimeSpan ResponseTime => _lastResponseTime;

        public async Task<string> GetHeader()
        {
            if (_lastResponse != null && _dateService.Now >= _expiryTime)
            {
                try
                {
                    var response = await _postUrlEncodedService
                                            .GetPostResult<Oauth2PasswordResponse>(_tokenUri,
                                                                                    new Dictionary<string, string>()
                                                                                    {
                                                                                        { "client_id", _clientId },
                                                                                        { "client_secret", _clientSecret },
                                                                                        { "grant_type", "refresh_token" },
                                                                                        { "refresh_token", _lastResponse.RefreshToken }
                                                                                    });
                    _lastResponse = response.response;
                    _lastResponseTime = response.responseTime;

                    _expiryTime = _dateService.Now.AddSeconds(_lastResponse.ExpiresIn - 5);
                } catch (Exception e)
                {
                    //assumes this is due to refresh token expiry - may be wrong
                    _lastResponse = null;
                    throw e;
                }
            }

            if (_lastResponse == null)
            {
                var response = await _postUrlEncodedService
                                        .GetPostResult<Oauth2PasswordResponse>(_tokenUri,
                                                                                new Dictionary<string, string>()
                                                                                {
                                                                                    { "client_id", _clientId },
                                                                                    { "client_secret", _clientSecret },
                                                                                    { "grant_type", "password" },
                                                                                    { "username", _userName },
                                                                                    { "password", _password }
                                                                                });
                _lastResponse = response.response;
                _lastResponseTime = response.responseTime;

                _expiryTime = _dateService.Now.AddSeconds(_lastResponse.ExpiresIn - 5);
            }

            return $"{_lastResponse.TokenType} {_lastResponse.AccessToken}";
        }
    }
}
