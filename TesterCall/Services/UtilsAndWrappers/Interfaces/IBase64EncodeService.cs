using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.UtilsAndWrappers.Interfaces
{
    public interface IBase64EncodeService
    {
        string Encode(params string[] args);
    }
}
