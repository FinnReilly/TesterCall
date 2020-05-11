using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class ResponseContentServiceFactory : IResponseContentServiceFactory
    {
        public IReadResponseContentService GetService(Type type)
        {
            //TO DO other non-default conditions
            if (type == null)
            {
                return new ReadNoContentService();
            }

            if (type == typeof(object))
            {
                return new ReadJsonToUnknownTypeService();
            }

            return new ReadJsonResponseContentService(type);
        }
    }
}
