using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Enums;
using TesterCall.Models.Endpoints;

namespace TesterCall.Services.Usage.Interfaces
{
    public interface IEndpointSearchService
    {
        IEnumerable<Endpoint> ByShortName(string searchString,
                                            string apiTagSearchString);
        IEnumerable<Endpoint> ByPathAndMethod(string searchString,
                                Method? method,
                                string apiTagSearchString);
    }
}
