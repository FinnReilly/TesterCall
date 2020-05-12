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

namespace TesterCall.Tests.Services.Generation.JsonExtraction.OpenApiUmbrellaJsonTypeParserTests
{
    [TestClass]
    // nb this class also tests JsonCatchAllTypeModel extensions
    public class ParseTests
    {
        private Mock<IOpenApiSpecObjectParser> _objectParser;

        private OpenApiUmbrellaJsonTypeParser _service;

        private JsonCatchAllTypeModel _primitiveModel;
        private JsonCatchAllTypeModel _enumModel;
        private JsonCatchAllTypeModel _objectModel;
        private JsonCatchAllTypeModel _referenceModel;
        private JsonCatchAllTypeModel _arrayModel;
        private JsonCatchAllTypeModel _objectProp;
        private JsonCatchAllTypeModel _allOfMember;
        private JsonCatchAllTypeModel _arrayItems;
        private List<string> _enumValues;
        private string _referenceValue;

        [TestInitialize]
        public void TestInitialise()
        {
            _objectParser = new Mock<IOpenApiSpecObjectParser>();

            _service = new OpenApiUmbrellaJsonTypeParser();

            _primitiveModel = new JsonCatchAllTypeModel()
            {
                Type = "integer",
                Format = "int64"
            };
            _enumValues = new List<string>()
            {
                "One",
                "Two",
                "Three"
            };
            _enumModel = new JsonCatchAllTypeModel()
            {
                Type = "string",
                Enum = _enumValues
            };
            _objectProp = new JsonCatchAllTypeModel();
            _allOfMember = new JsonCatchAllTypeModel();
            _objectModel = new JsonCatchAllTypeModel()
            {
                Type = "object",
                Properties = new Dictionary<string, JsonCatchAllTypeModel>()
                {
                    { "Property", _objectProp }
                },
                AllOf = new List<JsonCatchAllTypeModel>()
                {
                    _allOfMember
                }
            };
            _referenceValue = Guid.NewGuid().ToString();
            _referenceModel = new JsonCatchAllTypeModel()
            {
                Reference = _referenceValue
            };
            _arrayItems = new JsonCatchAllTypeModel()
            {
                Type = "object",
                Properties = new Dictionary<string, JsonCatchAllTypeModel>()
                {
                    {"ArrayMemberProperty", _objectProp }
                }
            };
            _arrayModel = new JsonCatchAllTypeModel()
            {
                Type = "array",
                Items = _arrayItems
            };

            _objectParser.Setup(s => s.Parse(It.IsAny<JsonCatchAllTypeModel>()))
                .Returns(() => new OpenApiObjectType());
        }

        [TestMethod]
        public void BehavesCorrectlyWithSimplePrimitiveType()
        {
            var output = _service.Parse(_objectParser.Object,
                                        _primitiveModel);

            output.GetType().Should().Be(typeof(OpenApiPrimitiveType));
            ((OpenApiPrimitiveType)output).Format.Should().Be("int64");
            ((OpenApiPrimitiveType)output).Type.Should().Be("integer");
        }

        [TestMethod]
        public void BehavesCorrectlyWithEnumType()
        {
            var output = _service.Parse(_objectParser.Object,
                                        _enumModel);

            output.GetType().Should().Be(typeof(OpenApiEnumType));
            ((OpenApiEnumType)output).Type.Should().Be("string");
            ((OpenApiEnumType)output).Enum.Should().BeEquivalentTo(_enumValues);
        }

        [TestMethod]
        public void BehavesCorrectlyWithReferenceType()
        {
            var output = _service.Parse(_objectParser.Object,
                                        _referenceModel);

            output.GetType().Should().Be(typeof(OpenApiReferencedType));
            ((OpenApiReferencedType)output).Type.Should().Be(_referenceValue);
        }

        [TestMethod]
        public void BehavesCorrectlyWithObjectType()
        {
            var output = _service.Parse(_objectParser.Object,
                                        _objectModel);

            _objectParser.Verify(s => s.Parse(_objectModel), Times.Once);
            output.GetType().Should().Be(typeof(OpenApiObjectType));
        }

        [TestMethod]
        public void BehavesCorrectlyWithObjectTypeAllOfOnly()
        {
            _objectModel.Properties = new Dictionary<string, JsonCatchAllTypeModel>();

            var output = _service.Parse(_objectParser.Object,
                                        _objectModel);

            _objectParser.Verify(s => s.Parse(_objectModel), Times.Once);
            output.GetType().Should().Be(typeof(OpenApiObjectType));
        }

        [TestMethod]
        public void BehavesCorrectlyWithObjectTypePropertiesOnly()
        {
            _objectModel.AllOf = new List<JsonCatchAllTypeModel>();

            var output = _service.Parse(_objectParser.Object,
                                        _objectModel);

            _objectParser.Verify(s => s.Parse(_objectModel), Times.Once);
            output.GetType().Should().Be(typeof(OpenApiObjectType));
        }

        [TestMethod]
        public void BehavesCorrectlyWithArrayModel()
        {
            var output = _service.Parse(_objectParser.Object,
                                        _arrayModel);

            _objectParser.Verify(s => s.Parse(_arrayItems), Times.Once);
            output.GetType().Should().Be(typeof(OpenApiArrayType));
            ((OpenApiArrayType)output).Items.GetType().Should().Be(typeof(OpenApiObjectType));
        }
    }
}
