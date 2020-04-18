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

namespace TesterCall.Tests.Services.AuthStrategies.Oauth2ClientCredentialsTests
{
    [TestClass]
    public class GetHeaderTests
    {
        private Mock<IDateTimeWrapper> _dateService;
        private Mock<IPostUrlFormEncodedService> _postUrlEncodedService;

        private Oauth2ClientCredentials _service;

        private string _tokenUri;
        private string _clientId;
        private string _clientSecret;
        private DateTime _expiryTime;
        private DateTime _beforeExpiry;
        private DateTime _afterExpiry;
        private Oauth2BaseResponse _returnedResponse;
        private string _returnedToken;

        [TestInitialize]
        public void TestInitialise()
        {
            _dateService = new Mock<IDateTimeWrapper>();
            _postUrlEncodedService = new Mock<IPostUrlFormEncodedService>();

            _tokenUri = Guid.NewGuid().ToString();
            _clientId = Guid.NewGuid().ToString();
            _clientSecret = Guid.NewGuid().ToString();
            _returnedToken = Guid.NewGuid().ToString();

            _service = new Oauth2ClientCredentials(_dateService.Object,
                                                    _postUrlEncodedService.Object,
                                                    _tokenUri,
                                                    _clientId,
                                                    _clientSecret);

            _expiryTime = 17.April(2020).At(13, 36);
            _beforeExpiry = 17.April(2020).At(11, 00);
            _afterExpiry = 17.April(2020).At(15, 00);

            _returnedResponse = new Oauth2BaseResponse()
            {
                AccessToken = _returnedToken,
                ExpiresIn = 100,
                TokenType = "Bearer"
            };

            _dateService.Setup(s => s.Now).Returns(_beforeExpiry);
            _postUrlEncodedService.Setup(s => s.GetPostResult<Oauth2BaseResponse>(_tokenUri,
                                                                                It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(_returnedResponse));
        }

        [TestMethod]
        public async Task BehavesAsExpectedOnFirstCall()
        {
            var output = await _service.GetHeader();

            _postUrlEncodedService.Verify(s => s.GetPostResult<Oauth2BaseResponse>(_tokenUri,
                                                                                    It.Is<Dictionary<string, string>>(dict => dict.ContainsKey("client_id")
                                                                                                                                && dict["client_id"] == _clientId
                                                                                                                                && dict.ContainsKey("client_secret")
                                                                                                                                && dict["client_secret"] == _clientSecret
                                                                                                                                && dict.ContainsKey("grant_type")
                                                                                                                                && dict["grant_type"] == "client_credentials")));
            output.Should().Be($"Bearer {_returnedToken}");
            _service.LastResponse.Should().Be(_returnedResponse);
        }

        [TestMethod]
        public async Task BehavesAsExpectedOnSecondCallWhenInDate()
        {
            await _service.GetHeader();
            var newResult = new Oauth2BaseResponse();
            var expectedHeader = $"Bearer {_returnedToken}";
            _postUrlEncodedService.Setup(s => s.GetPostResult<Oauth2BaseResponse>(_tokenUri,
                                                                                    It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(newResult));

            var output = await _service.GetHeader();

            output.Should().Be(expectedHeader);
            _service.LastResponse.Should().Be(_returnedResponse);
        }

        [TestMethod]
        public async Task BehavesAsExpectedOnSecondCallWhenOutOfDate()
        {
            await _service.GetHeader();
            var newToken = Guid.NewGuid().ToString();
            var newResult = new Oauth2BaseResponse()
            {
                AccessToken = newToken,
                TokenType = "Bearer"
            };
            var expectedHeader = $"Bearer {newToken}";
            _dateService.Setup(d => d.Now).Returns(_afterExpiry);
            _postUrlEncodedService.Setup(s => s.GetPostResult<Oauth2BaseResponse>(_tokenUri,
                                                                                    It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(newResult));

            var output = await _service.GetHeader();

            output.Should().Be(expectedHeader);
            _service.LastResponse.Should().Be(newResult);
        }
    }
}
