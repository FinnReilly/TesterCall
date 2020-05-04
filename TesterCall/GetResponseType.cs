using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using TesterCall.Models.Endpoints;
using TesterCall.Services.Usage;
using TesterCall.Services.Usage.Interfaces;
using TesterCall.Services.UtilsAndWrappers;

namespace TesterCall
{
    [Cmdlet(VerbsCommon.Get, "ResponseType")]
    [OutputType(typeof(object), typeof(string))]
    public class GetResponseType : Cmdlet
    {

        private IObjectCreator _objectCreator;

        [Parameter(Mandatory = true,
                    ValueFromPipeline = true,
                    ValueFromPipelineByPropertyName = true,
                    Position = 0)]
        public Endpoint Endpoint { get; set; }
        [Alias("Example")]
        [Parameter(HelpMessage = "Populate all fields, even when values are not specified")]
        public SwitchParameter ExampleMode { get; set; }
        [Alias("Json")]
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

            _objectCreator = new ObjectCreator(new EnumFromStringService());
        }

        protected override void ProcessRecord()
        {
            if (!Endpoint.SuccessContentExpected)
            {
                WriteError(new ErrorRecord(new ArgumentException($"Endpoint {Endpoint.ShortName} does not" +
                                                                $"expect a response body"),
                                            "No response body",
                                            ErrorCategory.InvalidArgument,
                                            Endpoint));
            }

            var createdType = _objectCreator.Create(Endpoint.SuccessResponseBody.Type,
                                                    ReplaceProperties,
                                                    Convert.ToBoolean(ExampleMode));

            if (Convert.ToBoolean(AsJson))
            {
                var jsonOut = JsonConvert.SerializeObject(createdType, 
                                                            Formatting.Indented);

                WriteObject(jsonOut);
            }
            else
            {
                WriteObject(createdType);
            }
        }
    }
}
