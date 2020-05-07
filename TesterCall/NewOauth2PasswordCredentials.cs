using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
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
    [Cmdlet(VerbsCommon.New, "Oauth2PasswordCredentials")]
    [OutputType(typeof(Oauth2PasswordCredentials))]
    public class NewOauth2PasswordCredentials : TesterCallCmdlet
    {
        private IDateTimeWrapper _dateService;
        private IPostUrlFormEncodedService _postUrlEncodedService;
        private IResponseRecorderService _responseRecorder;

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
        [Alias("User")]
        [Parameter(Mandatory = true,
                    Position = 3,
                    ValueFromPipelineByPropertyName = true)]
        public string UserName { get; set; }
        [Alias("Pwd")]
        [Parameter(Mandatory = true,
                    Position = 4,
                    ValueFromPipelineByPropertyName = true)]
        public string Password { get; set; }

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
                                                                        new HttpClientWrapper(),
                                                                        _dateService);
            }

            if (_responseRecorder == null)
            {
                _responseRecorder = new ResponseRecorderService();
            }
        }

        protected override void ProcessRecord()
        {
            var output = new Oauth2PasswordCredentials(_dateService,
                                                        _postUrlEncodedService,
                                                        _responseRecorder,
                                                        TokenUri,
                                                        ClientId,
                                                        ClientSecret,
                                                        UserName,
                                                        Password);

            Await(output.GetHeader(),
                    "Oauth 2 Password Credentials Token Request",
                    "In progress");

            WriteObject(output);
        }
    }
}
