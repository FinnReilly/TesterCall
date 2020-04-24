using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation;
using TesterCall.Services.UtilsAndWrappers;

namespace TesterCall.Tests.Services.Generation.OpenApiEnumToTypeServiceTests
{
    [TestClass]
    public class GetTypeTests
    {
        private ModuleBuilderProvider _moduleProvider;

        private OpenApiEnumToTypeService _service;

        private OpenApiEnumType _enumeration;
        private string _name;

        [TestInitialize]
        public void TestInitialise()
        {
            _moduleProvider = new ModuleBuilderProvider();

            _service = new OpenApiEnumToTypeService(_moduleProvider);

            _enumeration = new OpenApiEnumType();
            _name = Guid.NewGuid().ToString();
        }

        [TestMethod]
        public void ReturnsExpectedEnumType()
        {
            _enumeration.Enum = new string[] { "Cat", "Dog", "Rabbit" };

            var output = _service.GetType(_enumeration,
                                            _name);

            output.Name.Should().Be(_name);
            output.IsEnum.Should().BeTrue();
            output.IsEnumDefined("Cat").Should().BeTrue();
            output.IsEnumDefined("Dog").Should().BeTrue();
            output.IsEnumDefined("Rabbit").Should().BeTrue();
        }
    }
}
