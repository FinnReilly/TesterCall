using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Usage.Formatting.Interfaces
{
    public interface IFilePathFormattingService
    {
        string FormatPath(string path,
                            string pwd);
    }
}
