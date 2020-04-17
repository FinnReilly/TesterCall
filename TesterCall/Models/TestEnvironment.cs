using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Enums;

namespace TesterCall.Models
{
    public class TestEnvironment
    {
        public Protocol Protocol { get; set; }
        public string Host { get; set; }

        public string BaseUrl => $"{Protocol.ToString().ToLowerInvariant()}://{Host}";
    }
}
