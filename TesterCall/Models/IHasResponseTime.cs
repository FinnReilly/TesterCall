using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Models
{
    public interface IHasResponseTime
    {
        TimeSpan ResponseTime { get; }
    }
}
