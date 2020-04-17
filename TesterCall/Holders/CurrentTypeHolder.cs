using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Holders
{
    public static class CurrentTypeHolder
    {
        public static Dictionary<string, Type> Types { get; set; }
    }
}
