using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Services.Usage;
using TesterCall.Services.Usage.Formatting.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Tests.Services.Usage.ApiResponseServiceTests
{
    [TestClass]
    public class ReturnContentTests
    {
        public class TestType
        {

        }

        private Mock<IDateTimeWrapper> _dateTime;
        private Mock<IHttpClientWrapper> _client;
        private Mock<IResponseContentServiceFactory> _contentServiceFactory;

        private ApiResponseService _service;

        private Mock<IReadResponseContentService> _contentService;
        private Mock<IReadResponseContentService> _errorContentService;
        private Mock<HttpResponseMessage> _response;

        private DateTime _first;
        private DateTime _second;
        private TimeSpan _responseTime;
        private HttpStatusCode _statusCode;
        private bool _responseIsSuccess;
        private object _content;
        private HttpRequestMessage _request;

        [TestInitialize]
        public void TestInitialise()
        {
            _dateTime = new Mock<IDateTimeWrapper>();
            _client = new Mock<IHttpClientWrapper>();
            _contentServiceFactory = new Mock<IResponseContentServiceFactory>();

            _service = new ApiResponseService(_dateTime.Object,
                                                _client.Object,
                                                _contentServiceFactory.Object);

            _contentService = new Mock<IReadResponseContentService>();
            _errorContentService = new Mock<IReadResponseContentService>();
            _response = new Mock<HttpResponseMessage>();

            _first = 11.May(2020).At(12, 15, 00);
            _responseTime = new TimeSpan(00, 05, 00);
            _second = _first.Add(_responseTime);
            _responseIsSuccess = true;
            _content = new object();
            _request = new HttpRequestMessage();
            _statusCode = HttpStatusCode.OK;
            var dateTimeQueue = new Queue<DateTime>();
            dateTimeQueue.Enqueue(_first);
            dateTimeQueue.Enqueue(_second);

            _contentServiceFactory.Setup(s => s.GetService(typeof(TestType)))
                                    .Returns(() => _contentService?.Object).Verifiable();
            _contentServiceFactory.Setup(s => s.GetService(typeof(object)))
                                    .Returns(() => _errorContentService?.Object).Verifiable();
            _contentService.Setup(s => s.ReadContent(It.IsAny<HttpResponseMessage>()))
                            .Returns(() => Task.FromResult(_content)).Verifiable();
            _errorContentService.Setup(s => s.ReadContent(It.IsAny<HttpResponseMessage>()))
                            .Returns(() => Task.FromResult(_content)).Verifiable();
            _response.Object.StatusCode = _statusCode;
            _dateTime.Setup(d => d.Now).Returns(() => dateTimeQueue.Dequeue());
            _client.Setup(c => c.SendAsync(_request))
                .Returns(() => Task.FromResult(_response.Object)).Verifiable();
        }

        [TestMethod]
        public async Task MakesExpectedCallInStandardCase()
        {
            var output = await _service.ReturnContent(_request,
                                                        typeof(TestType),
                                                        typeof(object));

            _contentServiceFactory.Verify();
            _dateTime.Verify(d => d.Now, Times.Exactly(2));
            _client.Verify();
            _contentService.Verify(s => s.ReadContent(_response.Object), Times.Once);
            _errorContentService.Verify(s => s.ReadContent(_response.Object), Times.Never);

            output.Content.Should().Be(_content);
            output.StatusCode.Should().Be(_statusCode);
            output.TimeSent.Should().Be(_first);
            output.ResponseTime.Should().Be(_responseTime);

            Cleanup();
        }

        [TestMethod]
        public async Task MakesExpectedCallsInStandardFailureCase()
        {
            _response.Object.StatusCode = HttpStatusCode.InternalServerError;
            var expectedStatus = HttpStatusCode.InternalServerError;

            var output = await _service.ReturnContent(_request,
                                                       typeof(TestType),
                                                       typeof(object));

            _contentServiceFactory.Verify();
            _dateTime.Verify(d => d.Now, Times.Exactly(2));
            _client.Verify();
            _contentService.Verify(s => s.ReadContent(_response.Object), Times.Never);
            _errorContentService.Verify(s => s.ReadContent(_response.Object), Times.Once);

            output.Content.Should().Be(_content);
            output.StatusCode.Should().Be(expectedStatus);
            output.TimeSent.Should().Be(_first);
            output.ResponseTime.Should().Be(_responseTime);

            Cleanup();
        }

        [TestMethod]
        public async Task MakesExpectedCallsInSuccessCaseWithNoContentService()
        {
            _contentService = null;

            var output = await _service.ReturnContent(_request,
                                                        typeof(TestType),
                                                        typeof(object));

            _contentServiceFactory.Verify();
            _dateTime.Verify(d => d.Now, Times.Exactly(2));
            _client.Verify();
            _errorContentService.Verify(s => s.ReadContent(_response.Object), Times.Never);

            output.Content.Should().Be(null);
            output.StatusCode.Should().Be(_statusCode);
            output.TimeSent.Should().Be(_first);
            output.ResponseTime.Should().Be(_responseTime);

            Cleanup();
        }

        [TestMethod]
        public async Task MakesExpectedCallsInFailureCaseWithNoContentService()
        {
            _response.Object.StatusCode = HttpStatusCode.InternalServerError;
            var expectedStatus = HttpStatusCode.InternalServerError;
            _errorContentService = null;

            var output = await _service.ReturnContent(_request,
                                                       typeof(TestType),
                                                       typeof(object));

            _contentServiceFactory.Verify();
            _dateTime.Verify(d => d.Now, Times.Exactly(2));
            _client.Verify();
            _contentService.Verify(s => s.ReadContent(_response.Object), Times.Never);

            output.Content.Should().Be(null);
            output.StatusCode.Should().Be(expectedStatus);
            output.TimeSent.Should().Be(_first);
            output.ResponseTime.Should().Be(_responseTime);

            Cleanup();
        }

        private void Cleanup()
        {
            _request.Dispose();
        }
    }
}
