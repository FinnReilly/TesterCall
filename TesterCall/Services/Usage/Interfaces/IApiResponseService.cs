using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Models;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Interfaces
{
    public interface IApiResponseService
    {
        Task<ResponseContentModel> ReturnContent(HttpRequestMessage request,
                                                Type expectedType,
                                                Type expectedErrorType = null);
    }
}
