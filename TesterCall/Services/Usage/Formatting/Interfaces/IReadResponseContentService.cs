﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TesterCall.Services.Usage.Formatting.Interfaces
{
    public interface IReadResponseContentService
    {
        Task<object> ReadContent(HttpResponseMessage response);
    }
}
