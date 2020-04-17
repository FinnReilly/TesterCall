using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TesterCall.Services.Usage.AuthStrategies.Interfaces
{
    public interface IGetAuthorisationHeaderStrategy
    {
        Task<string> GetHeader();
    }
}
