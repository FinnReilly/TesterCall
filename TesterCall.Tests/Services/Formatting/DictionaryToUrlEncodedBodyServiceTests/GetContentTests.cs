using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Services.Usage.Formatting;

namespace TesterCall.Tests.Services.Formatting.DictionaryToUrlEncodedBodyServiceTests
{
    [TestClass]
    public class GetContentTests
    {
        private DictionaryToUrlEncodedBodyService _service;

        [TestInitialize]
        public void TestInitialize()
        {
            _service = new DictionaryToUrlEncodedBodyService();
        }

        [TestMethod]
        [DataRow(new string[] {"first;1", "second;2", "third;3"}, "first=1&second=2&third=3")]
        [DataRow(new string[] {"first;1", "second;2"}, "first=1&second=2")]
        [DataRow(new string[] {"justOne;true"}, "justOne=true")]
        [DataRow(new string[] { }, "")]
        public async Task ReturnsCorrectContent(string[] content, string expected)
        {
            var input = new Dictionary<string, string>();

            foreach (var pair in content)
            {
                var split = pair.Split(';');
                input[split[0]] = split[1];
            }

            var stringContent = _service.GetContent(input);
            var innerContent = await stringContent.ReadAsStringAsync();

            innerContent.Should().Be(expected);
        }
    }
}
