using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.UtilsAndWrappers;

namespace TesterCall.Tests.Services.UtilsAndWrappers.EnumFromStringServiceTests
{
    [TestClass]
    public class ConvertStringToTests
    {
        private EnumFromStringService _service;

        public enum TestEnum
        {
            Good,
            Better,
            Best
        }

        [TestInitialize]
        public void TestInitialise()
        {
            _service = new EnumFromStringService();
        }

        [TestMethod]
        [DataRow("Good", TestEnum.Good)]
        [DataRow("Best", TestEnum.Best)]
        public void ConvertsCorrectly(string input, TestEnum expected)
        {
            var actual = _service.ConvertStringTo<TestEnum>(input);

            actual.Should().Be(expected);
        }
    }
}
