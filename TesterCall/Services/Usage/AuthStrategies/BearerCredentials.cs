using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Services.Usage.AuthStrategies.Interfaces;

namespace TesterCall.Services.Usage.AuthStrategies
{
    public class BearerCredentials : IGetAuthorisationHeaderStrategy
    {
        private readonly string _bearerToken;

        public BearerCredentials(string bearerToken)
        {
            _bearerToken = bearerToken;
        }

        public Task<string> GetHeader()
        {
            return Task.FromResult($"Bearer {_bearerToken}");
        }
    }
}
