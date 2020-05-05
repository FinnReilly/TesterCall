using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Models
{
    public class ResponseContentModel : IHasResponseTime
    {
        private readonly TimeSpan _responseTime;
        private readonly object _content;

        public ResponseContentModel(TimeSpan responseTime,
                                    object content)
        {
            _responseTime = responseTime;
            _content = content;
        }

        public TimeSpan ResponseTime => _responseTime;
        public object Content => _content;
    }
}
