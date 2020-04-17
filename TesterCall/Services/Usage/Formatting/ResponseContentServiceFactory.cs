using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class ResponseContentServiceFactory : IResponseContentServiceFactory
    {
        public IReadReponseContentService<TContent> GetService<TContent>()
        {
            //TO DO other non-default conditions

            return new ReadJsonResponseContentService<TContent>();
        }
    }
}
