using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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

        private Mock<IDictionaryToUrlEncodedBodyService> _contentCreateService;
        private Mock<IResponseContentServiceFactory> _contentReaderFactory;
        private Mock<IHttpClientWrapper> _client;
        private Mock<IReadReponseContentService<TestOutput>> _returnedContentService;

        private PostUrlFormEncodedService _service;

        private string _url;
        private Dictionary<string, string> _content;
        private StringContent _convertedContent;
        private HttpResponseMessage _response;
        private TestOutput _finalResult;

        [TestInitialize]
        public void TestInitialize()
        {
            _contentCreateService = new Mock<IDictionaryToUrlEncodedBodyService>();
            _contentReaderFactory = new Mock<IResponseContentServiceFactory>();
            _client = new Mock<IHttpClientWrapper>();
            _returnedContentService = new Mock<IReadReponseContentService<TestOutput>>();

            _service = new PostUrlFormEncodedService(_contentCreateService.Object,
                                                    _contentReaderFactory.Object,
                                                    _client.Object);

            _url = "test.com";
            _content = new Dictionary<string, string>();
            _convertedContent = new StringContent("");
            _response = new HttpResponseMessage();
        }
    }
}
