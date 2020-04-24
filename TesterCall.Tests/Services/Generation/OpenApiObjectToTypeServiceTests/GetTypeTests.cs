using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Models.OpenApi.Interfaces;
using TesterCall.Services.Generation;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.UtilsAndWrappers;

namespace TesterCall.Tests.Services.Generation.OpenApiObjectToTypeServiceTests
{
    [TestClass]
    public class GetTypeTests
    {
        private Mock<IOpenApiUmbrellaTypeResolver> _typeResolver;
        private Mock<IStealFieldsFromOpenApiObjectTypesService> _fieldStealer;
        private Mock<IObjectsProcessingKeyStore> _objectsKeyStore;
        private ModuleBuilderProvider _moduleBuilderProvider;

        private OpenApiObjectToTypeService _service;

        private OpenApiObjectType _inputType;
        private OpenApiDefinitionsModel _inputDefinitions;
        private OpenApiPrimitiveType _prop1;
        private OpenApiPrimitiveType _prop2;
        private string _name;

        [TestInitialize]
        public void TestInitialise()
        {
            _typeResolver = new Mock<IOpenApiUmbrellaTypeResolver>();
            _fieldStealer = new Mock<IStealFieldsFromOpenApiObjectTypesService>();
            _objectsKeyStore = new Mock<IObjectsProcessingKeyStore>();
            _moduleBuilderProvider = new ModuleBuilderProvider();

            _service = new OpenApiObjectToTypeService(_typeResolver.Object,
                                                        _fieldStealer.Object,
                                                        _objectsKeyStore.Object,
                                                        _moduleBuilderProvider);

            _prop1 = new OpenApiPrimitiveType();
            _prop2 = new OpenApiPrimitiveType();
            _inputType = new OpenApiObjectType()
            {
                Properties = new Dictionary<string, IOpenApiType>()
                {
                    { "FirstProperty", _prop1 },
                    { "SecondProperty", _prop2 }
                }
            };
            _inputDefinitions = new OpenApiDefinitionsModel();
            _name = Guid.NewGuid().ToString();

            _typeResolver.Setup(s => s.GetType(_service,
                                                It.IsAny<OpenApiPrimitiveType>(),
                                                _inputDefinitions,
                                                It.IsAny<string>()))
                        .Returns(typeof(int));
        }

        [TestMethod]
        public void BehavesCorrectlyInSimpleCase()
        {
            var output = _service.GetType(_inputType,
                                            _inputDefinitions,
                                            _name);

            _objectsKeyStore.Verify(s => s.ThrowIfPresent(_name), Times.Once);
            _objectsKeyStore.Verify(s => s.AddPresent(_name), Times.Once);
            _typeResolver.Verify(s => s.GetType(_service,
                                                _prop1,
                                                _inputDefinitions,
                                                $"{_name}_FirstProperty"), Times.Once);
            _typeResolver.Verify(s => s.GetType(_service,
                                                _prop2,
                                                _inputDefinitions,
                                                $"{_name}_SecondProperty"), Times.Once);
            _fieldStealer.Verify(s => s.AddFields(_service,
                                                    It.IsAny<TypeBuilder>(),
                                                    It.IsAny<IEnumerable<IOpenApiType>>(),
                                                    _inputDefinitions), Times.Never);
            _objectsKeyStore.Verify(s => s.RemovePresent(_name), Times.Once);

            output.Name.Should().Be(_name);
            output.GetProperties().Where(p => p.Name == "FirstProperty"
                                            || p.Name == "SecondProperty"
                                            && p.PropertyType == typeof(int));
        }

        [TestMethod]
        public void CallsFieldStealerWhenAllOfPopulated()
        {
            var allOf = new List<OpenApiObjectType>()
            {
                new OpenApiObjectType()
            };
            _inputType.AllOf = allOf;

            _service.GetType(_inputType,
                            _inputDefinitions,
                            _name);

            _fieldStealer.Verify(s => s.AddFields(_service,
                                                    It.IsAny<TypeBuilder>(),
                                                    allOf,
                                                    _inputDefinitions), Times.Once);
        }
    }
}
