using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Holders
{
    public static class GlobalServiceHolder
    {
        public static Dictionary<Type, object> Services { get; set; }
    }
}
