using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using TesterCall.Enums;
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
        private const string SpecParameterSet = "FromSpec";
        private const string PathParameterSet = "FromPath";

        private IInvokeOpenApiEndpointService _invocationService;
        private IObjectCreator _objectCreator;
        private IHttpClientWrapper _clientWrapper;

        [Parameter(Mandatory = true,
                    ValueFromPipeline = true,
                    ValueFromPipelineByPropertyName = true,
                    Position = 0,
                    ParameterSetName = SpecParameterSet)]
        public Endpoint Endpoint { get; set; }

        [Parameter(Mandatory = true,
                    ValueFromPipeline = true,
                    ValueFromPipelineByPropertyName = true,
                    Position = 0,
                    ParameterSetName = PathParameterSet)]
        public string Url { get; set; }

        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 1,
                    ParameterSetName = PathParameterSet)]
        public Method Method { get; set; }

        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    ParameterSetName = SpecParameterSet)]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    ParameterSetName = PathParameterSet)]
        public TestEnvironment Environment { get; set; }

        [Alias("Path")]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 2,
                    ParameterSetName = PathParameterSet)]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 1,
                    ParameterSetName = SpecParameterSet)]
        public Hashtable PathParams { get; set; }

        [Alias("Query")]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 2,
                    ParameterSetName = SpecParameterSet)]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 3,
                    ParameterSetName = PathParameterSet)]
        public Hashtable QueryParams { get; set; }

        [Alias("Header")]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 3,
                    ParameterSetName = SpecParameterSet)]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 4,
                    ParameterSetName = PathParameterSet)]
        public Hashtable HeaderParams { get; set; }

        [Alias("Body")]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 4,
                    HelpMessage = "This may be an object of the correct type or an object property map",
                    ParameterSetName = SpecParameterSet)]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 5,
                    HelpMessage = "This may be an object of the correct type or an object property map",
                    ParameterSetName = PathParameterSet)]
        public object RequestBody { get; set; }

        [Alias("Auth")]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 5,
                    ParameterSetName = SpecParameterSet)]
        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 6,
                    ParameterSetName = PathParameterSet)]
        public IGetAuthorisationHeaderStrategy AuthStrategy { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = PathParameterSet)]
        [Parameter(Mandatory = false, ParameterSetName = SpecParameterSet)]
        public SwitchParameter DeserialiseErrors { get; set; }

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
            var jsonContentService = new AddJsonContentToRequestService();
            var uriService = new UriGenerateService(addPathParamsService, addQueryParamsService);
            var createMessageService = new CreateMessageService(uriService,
                                                                addContentServiceFactory,
                                                                addHeaderParamsService);

            if (_invocationService == null)
            {
                _invocationService = new InvokeOpenApiEndpointService(apiResponseService,
                                                                        checkParametersService,
                                                                        createMessageService,
                                                                        responseRecorder,
                                                                        jsonContentService);
            }

            if (_objectCreator == null)
            {
                _objectCreator = new ObjectCreator(new EnumFromStringService());
            }
        }

        protected override void ProcessRecord()
        {
            var body = RequestBody;
            if (body != null)
            {
                var bodyType = body.GetType();
                if (bodyType == typeof(Hashtable) || 
                    bodyType == typeof(Hashtable[]) ||
                    bodyType == typeof(IEnumerable<>))
                {
                    body = _objectCreator.Create(Endpoint.RequestBody.Type, 
                                                body);
                }
            }

            var result = AwaitResult(_invocationService.InvokeEndpoint(Endpoint,
                                                                        Environment,
                                                                        QueryParams,
                                                                        PathParams,
                                                                        HeaderParams,
                                                                        AuthStrategy,
                                                                        body,
                                                                        DeserialiseErrors.ToBool()),
                                    $"Invoke Endpoint {Endpoint.ShortName}",
                                    $"Awaiting response from {Endpoint.Path}");

            WriteObject(result);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();

            if (_clientWrapper != null)
            {
                _clientWrapper.Dispose();
            }
        }
    }
}
