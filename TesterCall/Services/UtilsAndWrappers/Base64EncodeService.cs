using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.UtilsAndWrappers
{
    public class Base64EncodeService : IBase64EncodeService
    {
        public string Encode(params string[] args)
        {
            var stringToEncode = "";
            foreach (var arg in args)
            {
                stringToEncode += arg;
            }
            var bytes = Encoding.UTF8.GetBytes(stringToEncode);

            return Convert.ToBase64String(bytes);
        }
    }
}
