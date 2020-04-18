using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Services.Usage;
using TesterCall.Services.Usage.Formatting.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Tests.Services.PostUrlFormEncodedServiceTests
{
    [TestClass]
    public class GetPostResultTests
    {
        public class TestOutput
        {

        }

        private Mock<IResponseContentServiceFactory> _contentReaderFactory;
        private Mock<IHttpClientWrapper> _client;
        private Mock<IReadReponseContentService<TestOutput>> _returnedContentService;

        private PostUrlFormEncodedService _service;

        private string _url;
        private Dictionary<string, string> _content;
        private HttpResponseMessage _response;
        private TestOutput _finalResult;

        [TestInitialize]
        public void TestInitialize()
        {
            _contentReaderFactory = new Mock<IResponseContentServiceFactory>();
            _client = new Mock<IHttpClientWrapper>();
            _returnedContentService = new Mock<IReadReponseContentService<TestOutput>>();

            _service = new PostUrlFormEncodedService(_contentReaderFactory.Object,
                                                    _client.Object);

            _url = "http://www.test.com";
            _content = new Dictionary<string, string>();
            _response = new HttpResponseMessage();
            _finalResult = new TestOutput();

            _client.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Returns(Task.FromResult(_response)).Verifiable();
            _contentReaderFactory.Setup(f => f.GetService<TestOutput>())
                .Returns(_returnedContentService.Object).Verifiable();
            _returnedContentService.Setup(s => s.ReadContent(_response))
                .Returns(Task.FromResult(_finalResult)).Verifiable();
        }

        [TestMethod]
        public async Task MakesExpectedCallsAndReturns()
        {
            var result = await _service.GetPostResult<TestOutput>(_url,
                                                                    _content);

            _client.Verify();
            _contentReaderFactory.Verify();
            _returnedContentService.Verify();

            result.Should().Be(_finalResult);
        }
    }
}
