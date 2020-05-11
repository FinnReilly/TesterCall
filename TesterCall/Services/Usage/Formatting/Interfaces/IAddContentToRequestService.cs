using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TesterCall.Models.Endpoints;

namespace TesterCall.Services.Usage.Formatting.Interfaces
{
    public interface IAddContentToRequestService
    {
        void AddContent(HttpRequestMessage message,
                        object model);
    }
}
