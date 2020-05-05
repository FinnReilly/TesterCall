using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using TesterCall.Holders;
using TesterCall.Models.Interfaces;
using TesterCall.Models.Stats;

namespace TesterCall
{
    [Cmdlet(VerbsCommon.Get, "Statistics")]
    [OutputType(typeof(StatsBin), typeof(IHasResponseTime))]
    public class GetStatistics : Cmdlet
    {
        [Parameter(Mandatory = false,
                    Position = 0,
                    ValueFromPipeline = true)]
        public string Search { get; set; }
        [Parameter(HelpMessage = "Separate stats by recording session name")]
        public SwitchParameter BySession { get; set; }

        protected override void ProcessRecord()
        {
            var returnBins = StatsBinHolder.Bins;

            if (!string.IsNullOrEmpty(Search))
            {
                returnBins = returnBins.Where(b => b.SessionName.Contains(Search))
                                        .ToList();
            }

            if (Convert.ToBoolean(BySession))
            {
                WriteObject(returnBins, enumerateCollection: true);
            }
            else
            {
                WriteObject(returnBins.SelectMany(b => b.Responses),
                            enumerateCollection: true);
            }
        }
    }
}
