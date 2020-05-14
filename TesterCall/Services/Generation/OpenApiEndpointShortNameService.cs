﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Generation
{
    public class OpenApiEndpointShortNameService : IOpenApiEndpointShortNameService
    {
        private readonly ILastTokenInPathService _lastTokenService;

        public OpenApiEndpointShortNameService(ILastTokenInPathService lastTokenInPathService)
        {
            _lastTokenService = lastTokenInPathService;
        }

        public void CreateOrUpdateShortNames(IEnumerable<OpenApiEndpointModel> endpoints)
        {
            foreach (var endpoint in endpoints)
            {
                if (!string.IsNullOrEmpty(endpoint.ShortName))
                {
                    endpoint.ShortName = $"{endpoint.Tags.FirstOrDefault()}{endpoint.ShortName}";
                }
                else
                {
                    var lastToken = Regex.Replace(_lastTokenService.GetLastToken(endpoint.Path),
                                                    "({|})",
                                                    "_");

                    endpoint.ShortName = $"{endpoint.Tags?.FirstOrDefault()?.Replace(" ","")}" +
                        $"{endpoint.Method}" +
                        $"{lastToken}";
                }
            }
        }
    }
}
