using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models;
using TesterCall.Models.Interfaces;

namespace TesterCall.Services.UtilsAndWrappers.Interfaces
{
    public interface IResponseRecorderService
    {
        void RecordIfRequired(IHasResponseTime response);
    }
}
