using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Enums;
using TesterCall.Models;

namespace TesterCall.Tests.Models
{
    [TestClass]
    public class TestEnvironmentTests
    {
        [TestMethod]
        [DataRow(Protocol.HTTP, "testHost1", "http://testHost1")]
        [DataRow(Protocol.HTTPS, "www.testHost2.com", "https://www.testHost2.com")]
        public void BaseUrlGeneratedCorrectly(Protocol protocol,
                                            string hostName,
                                            string expectedUrl)
        {
            var env = new TestEnvironment()
            {
                Protocol = protocol,
                Host = hostName
            };

            env.BaseUrl.Should().Be(expectedUrl);
        }
    }
}
