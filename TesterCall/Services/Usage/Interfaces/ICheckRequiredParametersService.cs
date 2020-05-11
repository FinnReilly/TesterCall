using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.Endpoints;

namespace TesterCall.Services.Usage.Interfaces
{
    public interface ICheckRequiredParametersService
    {
        void CheckRequiredParametersPresent(Endpoint endpoint,
                                            IDictionary<string, string> queryParams,
                                            IDictionary<string, string> pathParams,
                                            IDictionary<string, string> headerParams);
    }
}
