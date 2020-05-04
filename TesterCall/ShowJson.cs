using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace TesterCall
{
    [Cmdlet(VerbsCommon.Show, "Json")]
    [OutputType(typeof(string))]
    public class ShowJson : Cmdlet
    {
        [Parameter(Mandatory = true,
                    ValueFromPipeline = true,
                    Position = 0)]
        public object Object { get; set; }

        protected override void ProcessRecord()
        {
            var output = JsonConvert.SerializeObject(Object);

            WriteObject(output);
        }
    }
}
