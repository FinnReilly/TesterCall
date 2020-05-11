﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Usage.Formatting.Interfaces
{
    public interface IResponseContentServiceFactory
    {
        IReadResponseContentService GetService(Type type);
    }
}
