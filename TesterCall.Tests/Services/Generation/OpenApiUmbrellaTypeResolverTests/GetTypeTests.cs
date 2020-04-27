using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation;
using TesterCall.Services.Generation.Interface;

namespace TesterCall.Tests.Services.Generation.OpenApiUmbrellaTypeResolverTests
{
    [TestClass]
    public class GetTypeTests
    {
        public class OpenApiUnsupportedType
        {

        }

        private Mock<IOpenApiPrimitiveToTypeService> _primitiveService;
        private Mock<IOpenApiReferenceToTypeService> _referenceService;
        private Mock<IOpenApiObjectToTypeService> _objectService;

        private OpenApiUmbrellaTypeResolver _service;

        private Dictionary<string, OpenApiObjectType> _definitions;
        private string _suggestedName;
        private OpenApiPrimitiveType _primitive;
        private OpenApiReferencedType _referenced;
        private OpenApiObjectType _object;
        private OpenApiArrayType _array;

        [TestInitialize]
        public void TestInitialise()
        {
            _primitiveService = new Mock<IOpenApiPrimitiveToTypeService>();
            _referenceService = new Mock<IOpenApiReferenceToTypeService>();
            _objectService = new Mock<IOpenApiObjectToTypeService>();

            _service = new OpenApiUmbrellaTypeResolver(_primitiveService.Object,
                                                        _referenceService.Object);

            _definitions = new Dictionary<string, OpenApiObjectType>();
            _suggestedName = Guid.NewGuid().ToString();
            _primitive = new OpenApiPrimitiveType();
            _referenced = new OpenApiReferencedType();
            _object = new OpenApiObjectType();
            _array = new OpenApiArrayType();

            _primitiveService.Setup(s => s.GetType(_primitive, It.IsAny<string>()))
                .Returns(typeof(int));
            _referenceService.Setup(s => s.GetType(_referenced, _definitions))
                .Returns(typeof(object));
            _objectService.Setup(s => s.GetType(_object, 
                                                _definitions, 
                                                It.IsAny<string>()))
                .Returns(typeof(object));
        }

        [TestMethod]
        public void BehavesCorrectlyForPrimitive()
        {
            var output = _service.GetType(_objectService.Object,
                                            _primitive,
                                            _definitions,
                                            _suggestedName);

            _primitiveService.Verify(s => s.GetType(_primitive, _suggestedName), 
                                    Times.Once);
            output.Should().Be(typeof(int));
        }

        [TestMethod]
        public void BehavesCorrectlyForReferenced()
        {
            var output = _service.GetType(_objectService.Object,
                                            _referenced,
                                            _definitions,
                                            _suggestedName);

            _referenceService.Verify(s => s.GetType(_referenced, _definitions),
                                    Times.Once);
            output.Should().Be(typeof(object));
        }

        [TestMethod]
        public void BehavesCorrectlyForObject()
        {
            var output = _service.GetType(_objectService.Object,
                                            _object,
                                            _definitions,
                                            _suggestedName);

            _objectService.Verify(s => s.GetType(_object,
                                                _definitions,
                                                _suggestedName), Times.Once);
            output.Should().Be(typeof(object));
        }

        [TestMethod]
        public void BehavesCorrectlyForObjectArray()
        {
            _array.Items = _object;
            var expectedSuggestedName = $"{_suggestedName}Member";
            var output = _service.GetType(_objectService.Object,
                                            _array,
                                            _definitions,
                                            _suggestedName);

            _objectService.Verify(s => s.GetType(_object,
                                                _definitions,
                                                expectedSuggestedName), Times.Once);
            output.Should().Be(typeof(IEnumerable<object>));
        }

        [TestMethod]
        public void BehavesCorrectlyForPrimitivesArray()
        {
            _array.Items = _primitive;
            var expectedSuggestedName = $"{_suggestedName}Member";
            var output = _service.GetType(_objectService.Object,
                                            _array,
                                            _definitions,
                                            _suggestedName);

            _primitiveService.Verify(s => s.GetType(_primitive,
                                                    expectedSuggestedName));
            output.Should().Be(typeof(IEnumerable<int>));
        }

        [TestMethod]
        public void BehavesCorrectlyForReferencedTypeArray()
        {
            _array.Items = _referenced;
            var output = _service.GetType(_objectService.Object,
                                            _array,
                                            _definitions,
                                            _suggestedName);

            _referenceService.Verify(s => s.GetType(_referenced,
                                                    _definitions));
            output.Should().Be(typeof(IEnumerable<object>));
        }

        [TestMethod]
        public void BehavesCorrectlyForArrayOfObjectArrays()
        {
            _array.Items = new OpenApiArrayType()
            {
                Items = _object
            };
            var expectedSuggestedName = $"{_suggestedName}MemberMember";

            var output = _service.GetType(_objectService.Object,
                                            _array,
                                            _definitions,
                                            _suggestedName);

            _objectService.Verify(s => s.GetType(_object,
                                                _definitions,
                                                expectedSuggestedName), Times.Once);
            output.Should().Be(typeof(IEnumerable<IEnumerable<object>>));
        }
    }
}
