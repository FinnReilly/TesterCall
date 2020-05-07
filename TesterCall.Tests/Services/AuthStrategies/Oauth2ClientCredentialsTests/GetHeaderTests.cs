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
        private Mock<IResponseRecorderService> _responseRecorder;

        private Oauth2ClientCredentials _service;

        private string _tokenUri;
        private string _clientId;
        private string _clientSecret;
        private DateTime _expiryTime;
        private DateTime _beforeExpiry;
        private DateTime _afterExpiry;
        private TimeSpan _responseTime;
        private Oauth2BaseResponse _returnedResponse;
        private (TimeSpan, Oauth2BaseResponse) _returnedTuple;
        private string _returnedToken;

        [TestInitialize]
        public void TestInitialise()
        {
            _dateService = new Mock<IDateTimeWrapper>();
            _postUrlEncodedService = new Mock<IPostUrlFormEncodedService>();
            _responseRecorder = new Mock<IResponseRecorderService>();

            _tokenUri = Guid.NewGuid().ToString();
            _clientId = Guid.NewGuid().ToString();
            _clientSecret = Guid.NewGuid().ToString();
            _returnedToken = Guid.NewGuid().ToString();

            _service = new Oauth2ClientCredentials(_dateService.Object,
                                                    _postUrlEncodedService.Object,
                                                    _responseRecorder.Object,
                                                    _tokenUri,
                                                    _clientId,
                                                    _clientSecret);

            _expiryTime = 17.April(2020).At(13, 36);
            _beforeExpiry = 17.April(2020).At(11, 00);
            _afterExpiry = 17.April(2020).At(15, 00);
            
            _responseTime = new TimeSpan(0, 0, 0, 0, 30);
            _returnedResponse = new Oauth2BaseResponse()
            {
                AccessToken = _returnedToken,
                ExpiresIn = 100,
                TokenType = "Bearer"
            };
            _returnedTuple = (_responseTime, _returnedResponse);

            _dateService.Setup(s => s.Now).Returns(_beforeExpiry);
            _postUrlEncodedService.Setup(s => s.GetPostResult<Oauth2BaseResponse>(_tokenUri,
                                                                                It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(_returnedTuple));
        }

        [TestMethod]
        public async Task BehavesAsExpectedOnFirstCall()
        {
            var expectedExpiry = _beforeExpiry.AddSeconds(95);

            var output = await _service.GetHeader();

            _postUrlEncodedService.Verify(s => s.GetPostResult<Oauth2BaseResponse>(_tokenUri,
                                                                                    It.Is<Dictionary<string, string>>(dict => dict.ContainsKey("client_id")
                                                                                                                                && dict["client_id"] == _clientId
                                                                                                                                && dict.ContainsKey("client_secret")
                                                                                                                                && dict["client_secret"] == _clientSecret
                                                                                                                                && dict.ContainsKey("grant_type")
                                                                                                                                && dict["grant_type"] == "client_credentials")));
            _responseRecorder.Verify(r => r.RecordIfRequired(It.Is<Oauth2BaseResponse>(resp => resp.ResponseTime == _responseTime)), Times.Once);
            output.Should().Be($"Bearer {_returnedToken}");
            _service.LastResponse.Should().Be(_returnedResponse);
            _service.ExpectedExpiry.Should().Be(expectedExpiry);
            _service.ResponseTime.Should().Be(_responseTime);
        }

        [TestMethod]
        public async Task BehavesAsExpectedOnSecondCallWhenInDate()
        {
            await _service.GetHeader();
            var newResult = new Oauth2BaseResponse();
            var newTuple = (_responseTime, newResult);
            var expectedHeader = $"Bearer {_returnedToken}";
            var expectedExpiry = _beforeExpiry.AddSeconds(95);
            _postUrlEncodedService.Setup(s => s.GetPostResult<Oauth2BaseResponse>(_tokenUri,
                                                                                    It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(newTuple));

            var output = await _service.GetHeader();

            _responseRecorder.Verify(r => r.RecordIfRequired(It.IsAny<Oauth2BaseResponse>()), Times.Once);
            output.Should().Be(expectedHeader);
            _service.LastResponse.Should().Be(_returnedResponse);
            _service.ExpectedExpiry.Should().Be(expectedExpiry);
            _service.ResponseTime.Should().Be(_responseTime);
        }

        [TestMethod]
        public async Task BehavesAsExpectedOnSecondCallWhenOutOfDate()
        {
            await _service.GetHeader();
            var newToken = Guid.NewGuid().ToString();
            var newResult = new Oauth2BaseResponse()
            {
                AccessToken = newToken,
                ExpiresIn = 100,
                TokenType = "Bearer"
            };
            var newTuple = (_responseTime, newResult);
            var expectedHeader = $"Bearer {newToken}";
            var expectedExpiry = _afterExpiry.AddSeconds(95);
            _dateService.Setup(d => d.Now).Returns(_afterExpiry);
            _postUrlEncodedService.Setup(s => s.GetPostResult<Oauth2BaseResponse>(_tokenUri,
                                                                                    It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(newTuple));

            var output = await _service.GetHeader();

            _responseRecorder.Verify(r => r.RecordIfRequired(It.IsAny<Oauth2BaseResponse>()), Times.Exactly(2));
            output.Should().Be(expectedHeader);
            _service.LastResponse.Should().Be(newResult);
            _service.ResponseTime.Should().Be(_responseTime);
            _service.ExpectedExpiry.Should().Be(expectedExpiry);
        }
    }
}
