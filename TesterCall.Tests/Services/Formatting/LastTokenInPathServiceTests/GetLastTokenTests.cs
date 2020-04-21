using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.Usage.Formatting;

namespace TesterCall.Tests.Services.Formatting.LastTokenInPathServiceTests
{
    [TestClass]
    public class GetLastTokenTests
    {
        LastTokenInPathService _service;

        [TestInitialize]
        public void TestInitialise()
        {
            _service = new LastTokenInPathService();
        }

        [TestMethod]
        [DataRow("#/components/targetName", "targetName")]
        [DataRow("www.something.com/api/Users/1/cards", "cards")]
        //fall through case
        [DataRow("string without slashes", "string without slashes")]
        //no empty strings
        [DataRow("something/target/", "target")]
        public void OutputsAsExpected(string input, string expected)
        {
            var result = _service.GetLastToken(input);

            result.Should().Be(expected);
        }
    }
}
