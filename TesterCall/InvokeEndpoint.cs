﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using TesterCall.Models;
using TesterCall.Models.Endpoints;
using TesterCall.Powershell;
using TesterCall.Services.Usage;
using TesterCall.Services.Usage.AuthStrategies.Interfaces;
using TesterCall.Services.Usage.Formatting;
using TesterCall.Services.Usage.Formatting.Interfaces;
using TesterCall.Services.Usage.Interfaces;
using TesterCall.Services.UtilsAndWrappers;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall
{
    [Cmdlet(VerbsLifecycle.Invoke, "Endpoint")]
    [OutputType(typeof(ResponseContentModel))]
    public class InvokeEndpoint : TesterCallCmdlet
    {
        private IInvokeOpenApiEndpointService _invocationService;
        private IHttpClientWrapper _clientWrapper;

        [Parameter(Mandatory = true,
                    ValueFromPipeline = true,
                    ValueFromPipelineByPropertyName = true,
                    Position = 0)]
        public Endpoint Endpoint { get; set; }
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true)]
        public TestEnvironment Environment { get; set; }
        [Alias("Path")]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 1)]
        public Hashtable PathParams { get; set; }
        [Alias("Query")]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 2)]
        public Hashtable QueryParams { get; set; }
        [Alias("Header")]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 3)]
        public Hashtable HeaderParams { get; set; }
        [Alias("Body")]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 4)]
        public object RequestBody { get; set; }
        [Alias("Auth")]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 5)]
        public IGetAuthorisationHeaderStrategy AuthStrategy { get; set; }
        [Alias("DeserialiseErrors")]
        [Parameter(Mandatory = false)]
        public SwitchParameter AttemptErrorResponseDeserialisation { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (_clientWrapper == null)
            {
                _clientWrapper = new HttpClientWrapper();
            }

            var apiResponseService = new ApiResponseService(new DateTimeWrapper(),
                                                            _clientWrapper,
                                                            new ResponseContentServiceFactory());
            var checkParametersService = new CheckRequiredParametersService();
            var responseRecorder = new ResponseRecorderService();
            var addContentServiceFactory = new AddContentServiceFactory(new AddJsonContentToRequestService());
            var addPathParamsService = new AddPathParamsService();
            var addQueryParamsService = new AddQueryParamsService();
            var addHeaderParamsService = new AddHeaderParamsService();
            var uriService = new UriGenerateService(addPathParamsService, addQueryParamsService);
            var createMessageService = new CreateMessageService(uriService,
                                                                addContentServiceFactory,
                                                                addHeaderParamsService);

            if (_invocationService == null)
            {
                _invocationService = new InvokeOpenApiEndpointService(apiResponseService,
                                                                        checkParametersService,
                                                                        createMessageService,
                                                                        responseRecorder);
            }
        }

        protected override void ProcessRecord()
        {
            var result = AwaitResult(_invocationService.InvokeEndpoint(Endpoint,
                                                                        Environment,
                                                                        QueryParams,
                                                                        PathParams,
                                                                        HeaderParams,
                                                                        AuthStrategy,
                                                                        RequestBody,
                                                                        AttemptErrorResponseDeserialisation.ToBool()),
                                    $"Invoke Endpoint {Endpoint.ShortName}",
                                    $"Awaiting response from {Endpoint.Path}");

            WriteObject(result);
        }
    }
}