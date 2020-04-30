using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Models.Endpoints
{
    public class Parameter
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool Required { get; set; }
    }
}
