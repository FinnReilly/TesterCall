using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TesterCall.Models.Endpoints;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class AddJsonContentToRequestService : IAddJsonContentToRequestService
    {
        public void AddContent(HttpRequestMessage message, 
                                object model)
        {
            message.Content = new StringContent(JsonConvert.SerializeObject(model,
                                                                            new JsonSerializerSettings() { 
                                                                                DateFormatHandling = DateFormatHandling.IsoDateFormat
                                                                            }),
                                                Encoding.UTF8,
                                                "application/json");
        }
    }
}
