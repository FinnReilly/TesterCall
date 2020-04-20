using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using TesterCall.Services.Usage.AuthStrategies;
using TesterCall.Services.UtilsAndWrappers;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall
{
    [Cmdlet(VerbsCommon.New, "BasicAuthCredentials")]
    [OutputType(typeof(BasicAuthCredentials))]
    public class NewBasicAuthCredentials : Cmdlet
    {
        private IBase64EncodeService _encodeService;

        [Alias("User")]
        [Parameter(Mandatory = true,
                    ValueFromPipelineByPropertyName = true,
                    Position = 0)]
        public string UserName { get; set; }
        [Alias("Pwd")]
        [Parameter(Mandatory = true,
                    ValueFromPipelineByPropertyName = true,
                    Position = 1)]
        public string Password { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (_encodeService == null)
            {
                _encodeService = new Base64EncodeService();
            }
        }

        protected override void ProcessRecord()
        {
            var output = new BasicAuthCredentials(_encodeService,
                                                    UserName,
                                                    Password);

            WriteObject(output);
        }
    }
}
