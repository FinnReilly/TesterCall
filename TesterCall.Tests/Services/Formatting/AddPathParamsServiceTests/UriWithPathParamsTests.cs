using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.Usage.Formatting;

namespace TesterCall.Tests.Services.Formatting.AddPathParamsServiceTests
{
    [TestClass]
    public class UriWithPathParamsTests
    {
        private AddPathParamsService _service;

        private string _defaultUrl;
        private Dictionary<string, string> _inputDictionary;

        [TestInitialize]
        public void TestInitialize()
        {
            _service = new AddPathParamsService();

            _defaultUrl = "test.com";
            _inputDictionary = new Dictionary<string, string>();
        }

        [TestMethod]
        public void ReturnsAsExpectedWithTwoValidParams()
        {
            _inputDictionary["thisIsATest"] = "true";
            _inputDictionary["hasMultipleParams"] = "also_true";
            var inputUrl = $"{_defaultUrl}/{{thisIsATest}}/{{hasMultipleParams}}";
            var expected = "test.com/true/also_true";

            var actual = _service.UriWithPathParams(inputUrl, _inputDictionary);

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void ReturnsAsExpectedwithSingleParam()
        {
            _inputDictionary["onlyHasOneParameter"] = "YES";
            var expected = "test.com/YES";
            var inputUrl = $"{_defaultUrl}/{{onlyHasOneParameter}}";

            var actual = _service.UriWithPathParams(inputUrl, _inputDictionary);

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void ReturnsAsExpectedWithNoParams()
        {
            var actual = _service.UriWithPathParams(_defaultUrl, _inputDictionary);

            actual.Should().Be(_defaultUrl);
        }

        [TestMethod]
        public void ReturnsAsExpectedWithNullParam()
        {
            var result = _service.UriWithPathParams(_defaultUrl, null);

            result.Should().Be(_defaultUrl);
        }

        [TestMethod]
        public void ReturnsAsExpectedWithOneValidParam()
        {
            _inputDictionary["thisIsATest"] = "true";
            _inputDictionary["hasMultipleParams"] = "also_true";
            var inputUrl = $"{_defaultUrl}/{{thisIsATest}}/hasMultipleParams";
            var expected = "test.com/true/hasMultipleParams";

            var actual = _service.UriWithPathParams(inputUrl, _inputDictionary);

            actual.Should().Be(expected);
        }
    }
}
