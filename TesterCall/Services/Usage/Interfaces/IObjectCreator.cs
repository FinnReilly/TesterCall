using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Usage.Interfaces
{
    public interface IObjectCreator
    {
        object Create(Type type, IDictionary<string, object> replaceFields = null);
    }
}
