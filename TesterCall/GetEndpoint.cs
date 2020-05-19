using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using TesterCall.Enums;
using TesterCall.Models.Endpoints;
using TesterCall.Services.Usage;
using TesterCall.Services.Usage.Interfaces;

namespace TesterCall
{
    [Cmdlet(VerbsCommon.Get, "Endpoint")]
    [OutputType(typeof(Endpoint))]
    public class GetEndpoint : Cmdlet
    {
        private IEndpointSearchService _searchService;

        [Parameter(Mandatory = true,
                    Position = 0,
                    ParameterSetName = "FromShortName")]
        public string ShortName { get; set; }
        [Parameter(Mandatory = true,
                    Position = 0, 
                    ParameterSetName = "FromPath")]
        public string Path { get; set; }
        [Parameter(Mandatory = false,
                    Position = 1,
                    ParameterSetName = "FromPath")]
        public Method? Method { get; set; }
        [Parameter(Mandatory = false,
                    Position = 1,
                    ParameterSetName = "FromShortName")]
        [Parameter(Mandatory = false,
                    Position = 2,
                    ParameterSetName = "FromPath")]
        public string ApiId { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (_searchService == null)
            {
                _searchService = new EndpointSearchService();
            }
        }

        protected override void ProcessRecord()
        {
            IEnumerable<Endpoint> endpoints = null;
            if (!string.IsNullOrEmpty(ShortName))
            {
                endpoints = _searchService.ByShortName(ShortName,
                                                        ApiId);
            }

            if (!string.IsNullOrEmpty(Path))
            {
                endpoints = _searchService.ByPathAndMethod(Path,
                                                            Method,
                                                            ApiId);
            }

            WriteObject(endpoints,
                        enumerateCollection: true);
        }
    }
}
