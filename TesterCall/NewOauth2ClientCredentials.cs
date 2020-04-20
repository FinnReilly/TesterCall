using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TesterCall.Powershell;
using TesterCall.Services.Usage;
using TesterCall.Services.Usage.AuthStrategies;
using TesterCall.Services.Usage.Formatting;
using TesterCall.Services.Usage.Interfaces;
using TesterCall.Services.UtilsAndWrappers;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall
{
    [Cmdlet(VerbsCommon.New, "Oauth2ClientCredentials")]
    [OutputType(typeof(Oauth2ClientCredentials))]
    public class NewOauth2ClientCredentials : TesterCallCmdlet
    {
        private IDateTimeWrapper _dateService;
        private IPostUrlFormEncodedService _postUrlEncodedService;

        [Alias("Uri")]
        [Parameter(Mandatory = true,
                    Position = 0,
                    ValueFromPipelineByPropertyName = true)]
        public string TokenUri { get; set; }
        [Alias("Client")]
        [Parameter(Mandatory = true,
                    Position = 1,
                    ValueFromPipelineByPropertyName = true)]
        public string ClientId { get; set; }
        [Alias("Secret")]
        [Parameter(Mandatory = true,
                    Position = 2,
                    ValueFromPipelineByPropertyName = true)]
        public string ClientSecret { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (_dateService == null)
            {
                _dateService = new DateTimeWrapper();
            }

            if (_postUrlEncodedService == null)
            {
                _postUrlEncodedService = new PostUrlFormEncodedService(new ResponseContentServiceFactory(),
                                                                        new HttpClientWrapper());
            }
        }

        protected override void ProcessRecord()
        {
            var output = new Oauth2ClientCredentials(_dateService,
                                                    _postUrlEncodedService,
                                                    TokenUri,
                                                    ClientId,
                                                    ClientSecret);

            Await(output.GetHeader(), 
                "Oauth 2 Client Credentials Token Request",
                "In progress");

            WriteObject(output);
        }
    }
}
