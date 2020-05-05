using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Holders;
using TesterCall.Models;
using TesterCall.Models.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.UtilsAndWrappers
{
    public class ResponseRecorderService : IResponseRecorderService
    {
        public void RecordIfRequired(IHasResponseTime response)
        {
            if (StatsBinHolder.Recording)
            {
                StatsBinHolder.Add(response);
            }
        }
    }
}
