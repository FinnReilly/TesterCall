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
    public class Oauth2PasswordCredentials : IGetAuthorisationHeaderStrategy
    {
        private readonly IDateTimeWrapper _dateService;
        private readonly IPostUrlFormEncodedService _postUrlEncodedService;

        private string _tokenUri;
        private string _clientId;
        private string _clientSecret;
        private string _userName;
        private string _password;
        private DateTime _expiryTime;
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

        public async Task<string> GetHeader()
        {
            if (_lastResponse != null && _dateService.Now >= _expiryTime)
            {
                try
                {
                    _lastResponse = await _postUrlEncodedService
                                            .GetPostResult<Oauth2PasswordResponse>(_tokenUri,
                                                                                    new Dictionary<string, string>()
                                                                                    {
                                                                                    { "client_id", _clientId },
                                                                                    { "client_secret", _clientSecret },
                                                                                    { "grant_type", "refresh_token" },
                                                                                    { "refresh_token", _lastResponse.RefreshToken }
                                                                                    });
                } catch (Exception e)
                {
                    //assumes this is due to refresh token expiry - may be wrong
                    _lastResponse = null;
                    throw e;
                }
            }

            if (_lastResponse == null)
            {
                _lastResponse = await _postUrlEncodedService
                                        .GetPostResult<Oauth2PasswordResponse>(_tokenUri,
                                                                                new Dictionary<string, string>()
                                                                                {
                                                                                    { "client_id", _clientId },
                                                                                    { "client_secret", _clientSecret },
                                                                                    { "grant_type", "password" },
                                                                                    { "username", _userName },
                                                                                    { "password", _password }
                                                                                });

                _expiryTime = _dateService.Now.AddSeconds(_lastResponse.ExpiresIn - 5);
            }

            return $"{_lastResponse.TokenType} {_lastResponse.AccessToken}";
        }
    }
}
