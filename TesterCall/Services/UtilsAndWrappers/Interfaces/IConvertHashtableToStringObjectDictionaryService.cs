using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.UtilsAndWrappers.Interfaces
{
    public interface IConvertHashtableToStringObjectDictionaryService
    {
        Dictionary<string, object> Convert(Hashtable hashtable);
    }
}
