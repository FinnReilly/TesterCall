using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.Usage.Formatting;

namespace TesterCall.Tests.Services.Formatting.AddQueryParamsServiceTests
{
    [TestClass]
    public class UriWithQueryTests
    {
        private AddQueryParamsService _service;
        private string _baseUrl;
        private Dictionary<string, string> _inputDictionary;

        [TestInitialize]
        public void TestInitialize()
        {
            _service = new AddQueryParamsService();

            _baseUrl = "test.com";
            _inputDictionary = new Dictionary<string, string>();
        }

        [TestMethod]
        public void ReturnsExpectedForOneParam()
        {
            _inputDictionary["singleParameter"] = "singleValue";
            var expectedUri = $"{_baseUrl}?singleParameter=singleValue";

            var actual = _service.UriWithQuery(_baseUrl, _inputDictionary);

            actual.Should().Be(expectedUri);
        }

        [TestMethod]
        public void ReturnsExpectedForNoParams()
        {
            var actual = _service.UriWithQuery(_baseUrl, _inputDictionary);

            actual.Should().Be(_baseUrl);
        }

        [TestMethod]
        public void ReturnsExpectedforMultipleParams()
        {
            _inputDictionary["FirstParam"] = "FirstValue";
            _inputDictionary["SecondParam"] = "SecondParam";
            var expectedUri = $"{_baseUrl}?FirstParam=FirstValue&SecondParam=SecondParam";

            var actual = _service.UriWithQuery(_baseUrl, _inputDictionary);

            actual.Should().Be(expectedUri);
        }
    }
}
