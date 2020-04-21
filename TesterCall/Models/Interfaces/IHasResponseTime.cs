using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Models.Interfaces
{
    public interface IHasResponseTime
    {
        TimeSpan ResponseTime { get; }
    }
}
