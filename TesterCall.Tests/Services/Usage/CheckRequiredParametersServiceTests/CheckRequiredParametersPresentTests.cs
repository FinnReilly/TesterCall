using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.Endpoints;
using TesterCall.Services.Usage;

namespace TesterCall.Tests.Services.Usage.CheckRequiredParametersServiceTests
{
    [TestClass]
    public class CheckRequiredParametersPresentTests
    {
        private CheckRequiredParametersService _service;

        private Endpoint _endpoint;
        private Dictionary<string, object> _query;
        private Dictionary<string, object> _path;
        private Dictionary<string, object> _header;

        [TestInitialize]
        public void TestInitialise()
        {
            _service = new CheckRequiredParametersService();

            _endpoint = new Endpoint()
            {
                QueryParameters = new List<Parameter>()
                {
                    new Parameter(){ Name = "TestQuery", Required = true},
                    new Parameter(){ Name = "TestQuery2"}
                },
                PathParameters = new List<Parameter>()
                {
                    new Parameter(){ Name = "TestPath", Required = true},
                    new Parameter(){ Name = "TestPath2", Required = true},
                    new Parameter(){ Name = "TestPath3"}
                },
                HeaderParameters = new List<Parameter>()
                {
                    new Parameter(){ Name = "TestHeader", Required = true}
                }
            };

            _query = new Dictionary<string, object>();
            _path = new Dictionary<string, object>();
            _header = new Dictionary<string, object>();
        }

        [TestMethod]
        public void AllMissingRequiredParametersIncludedInError()
        {
            var expectedError = "The following required parameters are missing: \n" +
                                "TestQuery in query \n" +
                                "TestPath in path \n" +
                                "TestPath2 in path \n" +
                                "TestHeader in header \n";

            _service.Invoking(s => s.CheckRequiredParametersPresent(_endpoint,
                                                                    _query,
                                                                    _path,
                                                                    _header))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(expectedError);
        }

        [TestMethod]
        public void SuppliedParametersRemovedFromError()
        {
            _query["TestQuery"] = "AnyValue";
            var expectedError = "The following required parameters are missing: \n" +
                                "TestPath in path \n" +
                                "TestPath2 in path \n" +
                                "TestHeader in header \n";

            _service.Invoking(s => s.CheckRequiredParametersPresent(_endpoint,
                                                                    _query,
                                                                    _path,
                                                                    _header))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(expectedError);
        }

        [TestMethod]
        public void NoMissingParametersNoError()
        {
            _query["TestQuery"] = "AnyValue";
            _path["TestPath"] = "AlsoAnyValue";
            _path["TestPath2"] = "It's not really about the values";
            _header["TestHeader"] = "As long as there's something there";

            _service.Invoking(s => s.CheckRequiredParametersPresent(_endpoint,
                                                                    _query,
                                                                    _path,
                                                                    _header))
                    .Should()
                    .NotThrow();
        }
    }
}
