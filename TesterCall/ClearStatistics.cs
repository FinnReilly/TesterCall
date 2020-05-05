using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using TesterCall.Holders;

namespace TesterCall
{
    [Cmdlet(VerbsCommon.Clear, "Statistics")]
    public class ClearStatistics : Cmdlet
    {
        protected override void ProcessRecord()
        {
            StatsBinHolder.FlushAll();
        }
    }
}
