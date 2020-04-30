using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using TesterCall.Models.Endpoints;
using TesterCall.Services.Usage;
using TesterCall.Services.Usage.Interfaces;

namespace TesterCall
{
    [Cmdlet(VerbsCommon.Get, "RequestType")]
    [OutputType(typeof(string), typeof(object))]
    public class GetRequestType : Cmdlet
    {
        private IObjectCreator _objectCreator;

        [Parameter(Mandatory = true,
                    ValueFromPipeline = true,
                    ValueFromPipelineByPropertyName = true,
                    Position = 0)]
        public Endpoint Endpoint { get; set; }

        [Parameter()]
        public SwitchParameter AsJson { get; set; }

        [Parameter(Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    Position = 1)]
        [Alias("Properties", "Props")]
        public Hashtable ReplaceProperties { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            _objectCreator = new ObjectCreator();
        }

        protected override void ProcessRecord()
        {
            var createdType = _objectCreator.Create(Endpoint.RequestBody.Type, 
                                                    ReplaceProperties);

            if (Convert.ToBoolean(AsJson))
            {
                var jsonOut = JsonConvert.SerializeObject(createdType, Formatting.Indented);

                WriteObject(jsonOut);
            }
            else
            {
                WriteObject(createdType);
            }
        }
    }
}
