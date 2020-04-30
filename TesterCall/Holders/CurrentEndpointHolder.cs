using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.Endpoints;

namespace TesterCall.Holders
{
    public static class CurrentEndpointHolder
    {
        public static Dictionary<string, Endpoint> Endpoints = new Dictionary<string, Endpoint>();
    }
}
