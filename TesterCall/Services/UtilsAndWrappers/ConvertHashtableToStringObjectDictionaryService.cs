using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.UtilsAndWrappers
{
    public class ConvertHashtableToStringObjectDictionaryService : IConvertHashtableToStringObjectDictionaryService
    {
        public Dictionary<string, object> Convert(Hashtable hashtable)
        {
            return hashtable.Cast<DictionaryEntry>()
                            .ToDictionary(kvp => (string)kvp.Key,
                                            kvp => kvp.Value);
        }
    }
}
