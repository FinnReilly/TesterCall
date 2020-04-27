using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.UtilsAndWrappers
{
    public class EnumFromStringService : IEnumFromStringService
    {
        public TEnum ConvertStringTo<TEnum>(string input)
            where TEnum : struct, Enum
        {
            if (!Enum.TryParse<TEnum>(input, out var outputEnum))
            {
                throw new NotSupportedException($"Could not parse {input} " +
                    $"as {nameof(TEnum)}");
            }

            return outputEnum; 
        }
    }
}
