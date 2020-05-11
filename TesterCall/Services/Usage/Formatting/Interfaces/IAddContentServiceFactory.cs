using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.Endpoints;

namespace TesterCall.Services.Usage.Formatting.Interfaces
{
    public interface IAddContentServiceFactory
    {
        IAddContentToRequestService GetService(Endpoint endpoint);
    }
}
