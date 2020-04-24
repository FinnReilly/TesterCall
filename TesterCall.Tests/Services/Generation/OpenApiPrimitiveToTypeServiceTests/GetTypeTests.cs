using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation;
using TesterCall.Services.Generation.Interface;

namespace TesterCall.Tests.Services.Generation.OpenApiPrimitiveToTypeServiceTests
{
    [TestClass]
    public class GetTypeTests
    {
        public enum ReturnedEnum
        {

        };

        private Mock<IOpenApiEnumToTypeService> _enumService;

        private OpenApiPrimitiveToTypeService _service;

        private OpenApiPrimitiveType _regularInputType;
        private OpenApiEnumType _enumInputType;
        private string _nameIfEnum;

        [TestInitialize]
        public void TestInitialise()
        {
            _enumService = new Mock<IOpenApiEnumToTypeService>();

            _service = new OpenApiPrimitiveToTypeService(_enumService.Object);

            _regularInputType = new OpenApiPrimitiveType();
            _enumInputType = new OpenApiEnumType();
            _nameIfEnum = Guid.NewGuid().ToString();

            _enumService.Setup(s => s.GetType(_enumInputType,
                                            _nameIfEnum))
                .Returns(typeof(ReturnedEnum)).Verifiable();
        }

        [TestMethod]
        public void ReturnsEnumWhereAppropriate()
        {
            var output = _service.GetType(_enumInputType,
                                            _nameIfEnum);

            _enumService.Verify();
            output.Should().Be(typeof(ReturnedEnum));
        }

        [TestMethod]
        [DataRow("string", "email", typeof(string))]
        [DataRow("string", null, typeof(string))]
        [DataRow("string", "date-time", typeof(DateTime))]
        [DataRow("integer", "", typeof(int))]
        [DataRow("float", "", typeof(double))]
        [DataRow("boolean", "", typeof(bool))]
        public void ReturnsOtherTypesWhereAppropriate(string type,
                                                        string format,
                                                        Type expected)
        {
            _regularInputType.Type = type;
            _regularInputType.Format = format;

            var output = _service.GetType(_regularInputType,
                                            _nameIfEnum);

            output.Should().Be(expected);
        }

        [TestMethod]
        public void ThrowsForUnsupportedTypes()
        {
            _regularInputType.Type = "hoobyNooby";
            _regularInputType.Format = "arbitrary-nonsense";
            var expectedError = "No support available for primitive with type = hoobyNooby" +
                " and format = arbitrary-nonsense";

            _service.Invoking(s => s.GetType(_regularInputType,
                                                _nameIfEnum))
                    .Should()
                    .Throw<NotSupportedException>()
                    .WithMessage(expectedError);
        }
    }
}
