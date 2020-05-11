using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.Endpoints;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class AddContentServiceFactory : IAddContentServiceFactory
    {
        private readonly IAddJsonContentToRequestService _jsonService;

        public AddContentServiceFactory(IAddJsonContentToRequestService addJsonContentToRequestService)
        {
            _jsonService = addJsonContentToRequestService;
        }

        public IAddContentToRequestService GetService(Endpoint endpoint)
        {
            if (endpoint.RequestBody?.ContentType == "application/json")
            {
                return _jsonService;
            }

            return _jsonService;
        }
    }
}
