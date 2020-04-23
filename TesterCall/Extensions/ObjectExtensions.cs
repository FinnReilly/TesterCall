using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Extensions
{
    public static class ObjectExtensions
    {
        public static bool Matches<T>(this object obj)
        {
            return obj.GetType() == typeof(T);
        }
    }
}
