using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.UtilsAndWrappers.Interfaces
{
    public interface IEnumFromStringService
    {
        TEnum ConvertStringTo<TEnum>(string input)
            where TEnum : struct, Enum;
    }
}
