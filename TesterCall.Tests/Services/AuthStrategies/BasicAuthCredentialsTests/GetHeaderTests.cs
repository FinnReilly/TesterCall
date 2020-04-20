using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Services.Usage.AuthStrategies;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Tests.Services.AuthStrategies.BasicAuthCredentialsTests
{
    [TestClass]
    public class GetHeaderTests
    {
        private Mock<IBase64EncodeService> _encodeService;

        private string _userName;
        private string _password;

        private BasicAuthCredentials _service;

        private string _base64EncodedString;

        [TestInitialize]
        public void TestInitialise()
        {
            _encodeService = new Mock<IBase64EncodeService>();

            _userName = Guid.NewGuid().ToString();
            _password = Guid.NewGuid().ToString();

            _service = new BasicAuthCredentials(_encodeService.Object,
                                                _userName,
                                                _password);

            _base64EncodedString = Guid.NewGuid().ToString();
            _encodeService.Setup(s => s.Encode(It.IsAny<string[]>()))
                .Returns(_base64EncodedString).Verifiable();
        }

        [TestMethod]
        public async Task BehavesAsExpected()
        {
            var expected = $"Basic {_base64EncodedString}";

            var actual = await _service.GetHeader();

            _encodeService.Verify();
            actual.Should().Be(expected);
        }
    }
}
