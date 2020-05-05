using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models;

namespace TesterCall.Services.UtilsAndWrappers.Interfaces
{
    public interface IResponseRecorderService
    {
        void RecordIfRequired(ResponseContentModel response);
    }
}
