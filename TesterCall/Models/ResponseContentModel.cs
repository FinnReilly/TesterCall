using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TesterCall.Models.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Models
{
    public class ResponseContentModel : IHasResponseTime
    {
        private readonly TimeSpan _responseTime;
        private readonly HttpStatusCode _status;
        private readonly DateTime _timeSent;
        private readonly object _content;

        public ResponseContentModel(TimeSpan responseTime,
                                    HttpStatusCode httpStatusCode,
                                    DateTime dateTime,
                                    object content)
        {
            _responseTime = responseTime;
            _status = httpStatusCode;
            _timeSent = dateTime;
            _content = content;
        }

        public TimeSpan ResponseTime => _responseTime;
        public HttpStatusCode StatusCode => _status;
        public DateTime TimeSent => _timeSent;
        public object Content => _content;
    }
}
