using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class DictionaryToUrlEncodedBodyService : IDictionaryToUrlEncodedBodyService
    {
        public StringContent GetContent(IDictionary<string, string> content)
        {
            var members = new List<string>();

            foreach(var pair in content)
            {
                members.Add($"{pair.Key}={pair.Value}");
            }

            return new StringContent(string.Join("&", members));
        }
    }
}
