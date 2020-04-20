using FluentAssertions;
using FluentAssertions.Extensions;
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
        private Mock<IDateTimeWrapper> _dateTime;
        private Mock<IReadReponseContentService<TestOutput>> _returnedContentService;

        private PostUrlFormEncodedService _service;

        private string _url;
        private Dictionary<string, string> _content;
        private HttpResponseMessage _response;
        private TestOutput _finalResult;
        private DateTime _now1;
        private DateTime _now2;
        private TimeSpan _expectedResponseTime;

        [TestInitialize]
        public void TestInitialize()
        {
            _contentReaderFactory = new Mock<IResponseContentServiceFactory>();
            _client = new Mock<IHttpClientWrapper>();
            _dateTime = new Mock<IDateTimeWrapper>();
            _returnedContentService = new Mock<IReadReponseContentService<TestOutput>>();

            _service = new PostUrlFormEncodedService(_contentReaderFactory.Object,
                                                    _client.Object,
                                                    _dateTime.Object);

            _url = "http://www.test.com";
            _content = new Dictionary<string, string>();
            _response = new HttpResponseMessage();
            _finalResult = new TestOutput();
            _now1 = 20.April(2020).At(12, 42);
            _now2 = 20.April(2020).At(12, 45);
            _expectedResponseTime = new TimeSpan(0, 3, 0);
            var timeQueue = new Queue<DateTime>();
            timeQueue.Enqueue(_now1);
            timeQueue.Enqueue(_now2);

            _client.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Returns(Task.FromResult(_response)).Verifiable();
            _contentReaderFactory.Setup(f => f.GetService<TestOutput>())
                .Returns(_returnedContentService.Object).Verifiable();
            _returnedContentService.Setup(s => s.ReadContent(_response))
                .Returns(Task.FromResult(_finalResult)).Verifiable();
            _dateTime.Setup(d => d.Now).Returns(() => timeQueue.Dequeue());
        }

        [TestMethod]
        public async Task MakesExpectedCallsAndReturns()
        {
            var expected = (_expectedResponseTime, _finalResult);

            var actual = await _service.GetPostResult<TestOutput>(_url,
                                                                    _content);

            _client.Verify();
            _contentReaderFactory.Verify();
            _returnedContentService.Verify();
            _dateTime.Verify(d => d.Now, Times.Exactly(2));

            actual.Should().Be(expected);
        }
    }
}
