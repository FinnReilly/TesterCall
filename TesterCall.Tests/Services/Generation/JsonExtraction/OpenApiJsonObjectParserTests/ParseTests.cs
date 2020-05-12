using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation.JsonExtraction;
using TesterCall.Services.Generation.JsonExtraction.Interfaces;
using TesterCall.Services.Generation.JsonExtraction.Models;

namespace TesterCall.Tests.Services.Generation.JsonExtraction.OpenApiJsonObjectParserTests
{
    [TestClass]
    public class ParseTests
    {
        private Mock<IOpenApiSpecUmbrellaTypeParser> _typeParser;

        private OpenApiJsonObjectParser _service;

        private JsonCatchAllTypeModel _inputModel;
        private JsonCatchAllTypeModel _prop1;
        private JsonCatchAllTypeModel _prop2;
        private JsonCatchAllTypeModel _extended1;
        private JsonCatchAllTypeModel _extended2;

        [TestInitialize]
        public void TestInitialise()
        {
            _typeParser = new Mock<IOpenApiSpecUmbrellaTypeParser>();

            _service = new OpenApiJsonObjectParser(_typeParser.Object);

            _prop1 = new JsonCatchAllTypeModel();
            _prop2 = new JsonCatchAllTypeModel();
            _extended1 = new JsonCatchAllTypeModel();
            _extended2 = new JsonCatchAllTypeModel();
            _inputModel = new JsonCatchAllTypeModel()
            {
                Properties = new Dictionary<string, JsonCatchAllTypeModel>()
                {
                    { "FirstProp", _prop1 },
                    { "SecondProp", _prop2 }
                },
                AllOf = new List<JsonCatchAllTypeModel>()
                {
                    _extended1,
                    _extended2
                }
            };

            _typeParser.Setup(s => s.Parse(_service,
                                            It.IsAny<JsonCatchAllTypeModel>()))
                .Returns(() => new OpenApiPrimitiveType());
        }

        [TestMethod]
        public void BehavesCorrectlyWhenAllOfIncluded()
        {
            var output = _service.Parse(_inputModel);

            _typeParser.Verify(s => s.Parse(_service, _prop1), Times.Once);
            _typeParser.Verify(s => s.Parse(_service, _prop2), Times.Once);
            _typeParser.Verify(s => s.Parse(_service, _extended1), Times.Once);
            _typeParser.Verify(s => s.Parse(_service, _extended2), Times.Once);

            output.GetType().Should().Be(typeof(OpenApiObjectType));
            output.Properties.Should().ContainKey("FirstProp");
            output.Properties["FirstProp"].GetType().Should().Be(typeof(OpenApiPrimitiveType));
            output.Properties.Should().ContainKey("SecondProp");
            output.Properties["SecondProp"].GetType().Should().Be(typeof(OpenApiPrimitiveType));
            output.AllOf.Should().HaveCount(2);
            output.AllOf.Should().AllBeOfType(typeof(OpenApiPrimitiveType));
        }

        [TestMethod]
        public void BehavesCorrectlyWhenNoAllOf()
        {
            _inputModel.AllOf = new List<JsonCatchAllTypeModel>();
            var output = _service.Parse(_inputModel);

            _typeParser.Verify(s => s.Parse(_service, _prop1), Times.Once);
            _typeParser.Verify(s => s.Parse(_service, _prop2), Times.Once);
            _typeParser.Verify(s => s.Parse(_service, _extended1), Times.Never);
            _typeParser.Verify(s => s.Parse(_service, _extended2), Times.Never);

            output.GetType().Should().Be(typeof(OpenApiObjectType));
            output.Properties.Should().ContainKey("FirstProp");
            output.Properties["FirstProp"].GetType().Should().Be(typeof(OpenApiPrimitiveType));
            output.Properties.Should().ContainKey("SecondProp");
            output.Properties["SecondProp"].GetType().Should().Be(typeof(OpenApiPrimitiveType));
            output.AllOf.Should().BeNull();
        }
    }
}
