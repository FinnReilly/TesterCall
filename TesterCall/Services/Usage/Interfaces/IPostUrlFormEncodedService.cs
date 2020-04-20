using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TesterCall.Services.Usage.Interfaces
{
    public interface IPostUrlFormEncodedService
    {
        Task<(TimeSpan responseTime, TPostResult response)> GetPostResult<TPostResult>(string uri, IDictionary<string, string> content);
    }
}
