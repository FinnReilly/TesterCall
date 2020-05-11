using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class FilePathFormattingService : IFilePathFormattingService
    {
        public string FormatPath(string path,
                                string pwd)
        {
            if (File.Exists(path))
            {
                return path;
            }

            var fullPath = Path.Combine(pwd, path);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Neither {path} nor {fullPath} was found");
            }

            return fullPath;
        }
    }
}
