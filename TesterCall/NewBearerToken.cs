using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using TesterCall.Services.Usage.AuthStrategies;

namespace TesterCall
{
    [Cmdlet(VerbsCommon.New, "BearerToken")]
    [OutputType(typeof(BearerCredentials))]
    public class NewBearerToken : Cmdlet
    {
        [Parameter(Mandatory = true,
                    Position = 0)]
        public string Token { get; set; }

        protected override void ProcessRecord()
        {
            var creds = new BearerCredentials(Token);

            WriteObject(creds);
        }
    }
}
