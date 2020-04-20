using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Models.Auth;
using TesterCall.Services.Usage.AuthStrategies;
using TesterCall.Services.Usage.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Tests.Services.AuthStrategies.Oauth2PasswordCredentialsTests
{
    [TestClass]
    public class GetHeaderTests
    { 
        private Mock<IDateTimeWrapper> _dateService;
        private Mock<IPostUrlFormEncodedService> _postUrlEncodedService;

        private string _tokenUri;
        private string _clientId;
        private string _clientSecret;
        private string _userName;
        private string _password;

        private Oauth2PasswordCredentials _service;

        private DateTime _beforeExpiry;
        private DateTime _afterExpiry;
        private string _returnedToken;
        private string _returnedRefreshToken;
        private TimeSpan _responseTime;
        private Oauth2PasswordResponse _returnedResponse;
        private (TimeSpan, Oauth2PasswordResponse) _returnedTuple;
        
        [TestInitialize]
        public void TestIntialiser()
        {
            _dateService = new Mock<IDateTimeWrapper>();
            _postUrlEncodedService = new Mock<IPostUrlFormEncodedService>();

            _tokenUri = Guid.NewGuid().ToString();
            _clientId = Guid.NewGuid().ToString();
            _clientSecret = Guid.NewGuid().ToString();
            _returnedToken = Guid.NewGuid().ToString();
            _returnedRefreshToken = Guid.NewGuid().ToString();

            _service = new Oauth2PasswordCredentials(_dateService.Object,
                                                        _postUrlEncodedService.Object,
                                                        _tokenUri,
                                                        _clientId,
                                                        _clientSecret,
                                                        _userName,
                                                        _password);

            _beforeExpiry = 18.April(2020).At(18, 41);
            _afterExpiry = 20.April(2020).At(20, 30);

            _responseTime = new TimeSpan(0, 0, 0, 0, 30);
            _returnedResponse = new Oauth2PasswordResponse()
            {
                AccessToken = _returnedToken,
                RefreshToken = _returnedRefreshToken,
                ExpiresIn = 100,
                TokenType = "Bearer"
            };
            _returnedTuple = (_responseTime, _returnedResponse);

            _dateService.Setup(s => s.Now).Returns(_beforeExpiry);
            _postUrlEncodedService.Setup(s => s.GetPostResult<Oauth2PasswordResponse>(_tokenUri,
                                                                                        It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(_returnedTuple));
        }

        [TestMethod]
        public async Task BehavesAsExpectedOnFirstCall()
        {
            var output = await _service.GetHeader();

            _postUrlEncodedService.Verify(s => s.GetPostResult<Oauth2PasswordResponse>(_tokenUri,
                                                                                        It.Is<Dictionary<string, string>>(dict => dict.ContainsKey("client_id")
                                                                                                                                && dict["client_id"] == _clientId
                                                                                                                                && dict.ContainsKey("client_secret")
                                                                                                                                && dict["client_secret"] == _clientSecret
                                                                                                                                && dict.ContainsKey("grant_type")
                                                                                                                                && dict["grant_type"] == "password"
                                                                                                                                && dict.ContainsKey("username")
                                                                                                                                && dict["username"] == _userName
                                                                                                                                && dict.ContainsKey("password")
                                                                                                                                && dict["password"] == _password)), Times.Once);
            _postUrlEncodedService.Verify(s => s.GetPostResult<Oauth2PasswordResponse>(_tokenUri,
                                                                                    It.Is<Dictionary<string, string>>(dict => dict.ContainsKey("refresh_token"))), Times.Never);
            output.Should().Be($"Bearer {_returnedToken}");
            _service.LastResponse.Should().Be(_returnedResponse);
        }

        [TestMethod]
        public async Task BehavesAsExpectedOnSecondCallWhenInDate()
        {
            await _service.GetHeader();

            var output = await _service.GetHeader();

            _postUrlEncodedService.Verify(s => s.GetPostResult<Oauth2PasswordResponse>(_tokenUri,
                                                                                        It.Is<Dictionary<string, string>>(dict => dict.ContainsKey("password")
                                                                                                                                && dict["password"] == _password)), Times.Once);
            _postUrlEncodedService.Verify(s => s.GetPostResult<Oauth2PasswordResponse>(_tokenUri,
                                                                                    It.Is<Dictionary<string, string>>(dict => dict.ContainsKey("refresh_token"))), Times.Never);
            output.Should().Be($"Bearer {_returnedToken}");
            _service.LastResponse.Should().Be(_returnedResponse);
        }

        [TestMethod]
        public async Task BehavesAsExpectedOnSecondCallWhenOutOfDate()
        {
            await _service.GetHeader();

            _dateService.Setup(s => s.Now).Returns(_afterExpiry);
            var newToken = Guid.NewGuid().ToString();
            _returnedResponse.AccessToken = newToken;
            var output = await _service.GetHeader();

            _postUrlEncodedService.Verify(s => s.GetPostResult<Oauth2PasswordResponse>(_tokenUri,
                                                                                       It.Is<Dictionary<string, string>>(dict => dict.ContainsKey("password")
                                                                                                                               && dict["password"] == _password)), Times.Once);
            _postUrlEncodedService.Verify(s => s.GetPostResult<Oauth2PasswordResponse>(_tokenUri,
                                                                                        It.Is<Dictionary<string, string>>(dict => dict.ContainsKey("client_id")
                                                                                                                                && dict["client_id"] == _clientId
                                                                                                                                && dict.ContainsKey("client_secret")
                                                                                                                                && dict["client_secret"] == _clientSecret
                                                                                                                                && dict.ContainsKey("grant_type")
                                                                                                                                && dict["grant_type"] == "refresh_token"
                                                                                                                                && dict.ContainsKey("refresh_token")
                                                                                                                                && dict["refresh_token"] == _returnedRefreshToken)), Times.Once);
            output.Should().Be($"Bearer {newToken}");
        }

        [TestMethod]
        public async Task BehavesCorrectlyInEventOfRefreshTokenFail()
        {
            await _service.GetHeader();

            _dateService.Setup(s => s.Now).Returns(_afterExpiry);
            var testException = new Exception();
            _postUrlEncodedService.Setup(s =>
                                    s.GetPostResult<Oauth2PasswordResponse>(_tokenUri,
                                                                            It.Is<Dictionary<string, string>>(dict => dict.ContainsKey("grant_type")
                                                                                                                    && dict["grant_type"] == "refresh_token")))
                .Throws(testException);
            var shouldBeTestException = new Exception();

            try 
            {
                await _service.GetHeader();
            }
            catch (Exception e)
            {
                shouldBeTestException = e;
            }

            _service.LastResponse.Should().BeNull();
            shouldBeTestException.Should().Be(testException);
        }
    }
}
