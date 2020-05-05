using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using TesterCall.Holders;

namespace TesterCall
{
    [Cmdlet(VerbsLifecycle.Enable, "Recording")]
    public class EnableRecording : Cmdlet
    {
        [Parameter(Mandatory = false,   
                    Position = 0)]
        public string SessionName { get; set; }

        protected override void ProcessRecord()
        {
            StatsBinHolder.StartRecording(SessionName);

            WriteInformation(new InformationRecord(StatsBinHolder.ActiveBin,
                                                    "Recording started"));
        }
    }
}
