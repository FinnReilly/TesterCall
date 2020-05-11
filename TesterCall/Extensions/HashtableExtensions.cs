using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesterCall.Extensions
{
    public static class HashtableExtensions
    {
        public static Dictionary<string, object> AsStringObjectDictionary(this Hashtable hashtable)
        {
            return hashtable?.Cast<DictionaryEntry>()
                            .ToDictionary(kvp => (string)kvp.Key,
                                            kvp => kvp.Value);
        }

        public static Dictionary<string, string> AsStringStringDictionary(this Hashtable hashtable)
        {
            return hashtable?.Cast<DictionaryEntry>()
                            .ToDictionary(kvp => (string)kvp.Key,
                                            kvp => (string)kvp.Value);
        }
    }
}
