using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using TesterCall.Holders;

namespace TesterCall
{
    [Cmdlet(VerbsLifecycle.Disable, "Recording")]
    public class DisableRecording : Cmdlet
    {
        protected override void ProcessRecord()
        {
            StatsBinHolder.StopRecording();

            WriteInformation(new InformationRecord(null,
                                                    "Recording Stopped"));
        }
    }
}
