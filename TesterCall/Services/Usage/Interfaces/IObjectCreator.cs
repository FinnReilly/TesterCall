using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Usage.Interfaces
{
    public interface IObjectCreator
    {
        object Create(Type type, Hashtable replaceFields = null);
    }
}
