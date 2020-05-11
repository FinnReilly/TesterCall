using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Models;
using TesterCall.Models.Endpoints;
using TesterCall.Services.Usage;
using TesterCall.Services.Usage.AuthStrategies.Interfaces;
using TesterCall.Services.Usage.Formatting.Interfaces;
using TesterCall.Services.Usage.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Tests.Services.Usage.InvokeOpenApiEndpointServiceTests
{
    [TestClass]
    public class InvokeEndpointTests
    {
        private Mock<IApiResponseService> _apiResponseService;
        private Mock<ICheckRequiredParametersService> _parameterCheckService;
        private Mock<ICreateMessageService> _createMessageService;
        private Mock<IResponseRecorderService> _responseRecorder;

        private Mock<IGetAuthorisationHeaderStrategy> _authStrategy;

        private InvokeOpenApiEndpointService _service;

        private ResponseContentModel _response;
        private Mock<HttpRequestMessage> _request;
        private Endpoint _endpoint;
        private TestEnvironment _testEnvironment;
        private Hashtable _queryParams;
        private Hashtable _pathParams;
        private Hashtable _headerParams;
        private object _requestBody;

        [TestInitialize]
        public void TestInitialise()
        {
            _apiResponseService = new Mock<IApiResponseService>();
            _parameterCheckService = new Mock<ICheckRequiredParametersService>();
            _createMessageService = new Mock<ICreateMessageService>();
            _responseRecorder = new Mock<IResponseRecorderService>();

            _authStrategy = new Mock<IGetAuthorisationHeaderStrategy>();

            _service = new InvokeOpenApiEndpointService(_apiResponseService.Object,
                                                        _parameterCheckService.Object,
                                                        _createMessageService.Object,
                                                        _responseRecorder.Object);

            _response = new ResponseContentModel(new TimeSpan(),
                                                HttpStatusCode.OK,
                                                new DateTime(),
                                                new object());
            _request = new Mock<HttpRequestMessage>();
            _endpoint = new Endpoint();
            _testEnvironment = new TestEnvironment();
            _queryParams = new Hashtable();
            _pathParams = new Hashtable();
            _headerParams = new Hashtable();
            _requestBody = new object();

            _createMessageService.Setup(s => s.CreateMessage(_endpoint,
                                                            _testEnvironment,
                                                            It.IsAny<IDictionary<string, string>>(),
                                                            It.IsAny<IDictionary<string, string>>(),
                                                            It.IsAny<IDictionary<string, string>>(),
                                                            _authStrategy.Object,
                                                            _requestBody))
                                .Returns(Task.FromResult(_request.Object))
                                .Verifiable();
            _apiResponseService.Setup(s => s.ReturnContent(_request.Object,
                                                            It.IsAny<Type>(),
                                                            It.IsAny<Type>())).Returns(Task.FromResult(_response));
        }

        [TestMethod]
        public async Task MakesExpectedCallsInDefaultCase()
        {
            var result = await _service.InvokeEndpoint(_endpoint,
                                                        _testEnvironment,
                                                        _queryParams,
                                                        _pathParams,
                                                        _headerParams,
                                                        _authStrategy.Object,
                                                        _requestBody);

            _parameterCheckService.Verify(s => s.CheckRequiredParametersPresent(_endpoint,
                                                                                It.IsAny<Dictionary<string, string>>(),
                                                                                It.IsAny<Dictionary<string, string>>(),
                                                                                It.IsAny<Dictionary<string, string>>()), Times.Once);
            _createMessageService.Verify();
            _apiResponseService.Verify(s => s.ReturnContent(_request.Object,
                                                            It.IsAny<Type>(),
                                                            null));
            result.Should().Be(_response);
        }

        [TestMethod]
        public async Task AnticipateObjectTypeWhenErrorDeserialisationRequested()
        {
            await _service.InvokeEndpoint(_endpoint,
                                        _testEnvironment,
                                        _queryParams,
                                        _pathParams,
                                        _headerParams,
                                        _authStrategy.Object,
                                        _requestBody,
                                        true);

            _apiResponseService.Verify(s => s.ReturnContent(_request.Object,
                                                            It.IsAny<Type>(),
                                                            typeof(object)));
        }
    }
}
