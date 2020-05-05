using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Holders;
using TesterCall.Models;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.UtilsAndWrappers
{
    public class ResponseRecorderService : IResponseRecorderService
    {
        public void RecordIfRequired(ResponseContentModel response)
        {
            if (StatsBinHolder.Recording)
            {
                StatsBinHolder.Add(response);
            }
        }
    }
}
