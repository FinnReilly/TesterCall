using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using TesterCall.Holders;
using TesterCall.Models;

namespace TesterCall
{
    [Cmdlet(VerbsCommon.Get, "DefaultTestEnvironment")]
    [OutputType(typeof(TestEnvironment))]
    public class GetDefaultTestEnvironment : Cmdlet
    {
        protected override void ProcessRecord()
        {
            var output = DefaultTestEnvironmentHolder.Environment;

            WriteObject(output);
        }
    }
}
