using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using TesterCall.Enums;
using TesterCall.Models;

namespace TesterCall
{
    [Cmdlet(VerbsCommon.New, "TestEnvironment")]
    [OutputType(typeof(TestEnvironment))]
    public class NewTestEnvironment : Cmdlet
    {
        [Parameter(Mandatory = true,
                    Position = 0,
                    HelpMessage = "Http/Https")]
        public Protocol Protocol { get; set; }
        [Parameter(Mandatory = true,
                    Position = 2,
                    HelpMessage = "Name of host/IP address")]
        public string Host { get; set; }

        protected override void ProcessRecord()
        {
            var output = new TestEnvironment()
            {
                Protocol = Protocol,
                Host = Host
            };

            WriteObject(output);
        }
    }
}
