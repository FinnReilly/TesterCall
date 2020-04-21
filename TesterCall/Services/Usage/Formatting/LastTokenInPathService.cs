using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class LastTokenInPathService : ILastTokenInPathService
    {
        public string GetLastToken(string input)
        {
            return input.Split('/').Last(t => !string.IsNullOrEmpty(t));
        }
    }
}
