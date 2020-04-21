using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.Interfaces;

namespace TesterCall.Models
{
    public class ResponseContentModel<TModel> : IHasResponseTime
    {
        private readonly TimeSpan _responseTime;
        private readonly TModel _content;

        public ResponseContentModel(TimeSpan responseTime,
                                    TModel content)
        {
            _responseTime = responseTime;
            _content = content;
        }

        public TimeSpan ResponseTime => _responseTime;
        public TModel Content => _content;
    }
}
