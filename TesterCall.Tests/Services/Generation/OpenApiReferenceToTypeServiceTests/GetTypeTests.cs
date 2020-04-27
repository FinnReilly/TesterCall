using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Holders;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Tests.Services.Generation.OpenApiReferenceToTypeServiceTests
{
    [TestClass]
    public class GetTypeTests
    {
        public class ExistingType
        {

        }
        public class NewType
        {

        }

        private Mock<ILastTokenInPathService> _lastTokenService;
        private Mock<IObjectsProcessingKeyStore> _objectKeyStore;
        private Mock<IOpenApiObjectToTypeService> _objectService;

        private OpenApiReferenceToTypeService _service;

        private OpenApiReferencedType _inputType;
        private OpenApiObjectType _definedNotCreated;
        private Dictionary<string, OpenApiObjectType> _inputDefinitions;

        private string _originalName;
        private string _processedName;

        [TestInitialize]
        public void TestInitialise()
        {
            _lastTokenService = new Mock<ILastTokenInPathService>();
            _objectKeyStore = new Mock<IObjectsProcessingKeyStore>();
            _objectService = new Mock<IOpenApiObjectToTypeService>();

            _service = new OpenApiReferenceToTypeService(_lastTokenService.Object,
                                                        _objectKeyStore.Object,
                                                        _objectService.Object);

            _inputType = new OpenApiReferencedType();
            _inputDefinitions = new Dictionary<string, OpenApiObjectType>();
            _inputDefinitions["NewType"] = _definedNotCreated;
            _originalName = "path/ExistingType";
            _processedName = "ExistingType";
            CurrentTypeHolder.Types["ExistingType"] = typeof(ExistingType);

            _lastTokenService.Setup(s => s.GetLastToken(It.IsAny<string>()))
                .Returns(() => _processedName).Verifiable();
            _objectService.Setup(s => s.GetType(_definedNotCreated,
                                                _inputDefinitions,
                                                "NewType"))
                            .Returns(typeof(NewType))
                            .Verifiable();
        }

        [TestMethod]
        public void BehavesAsExpectedWhenTypeExists()
        {
            _inputType.Type = _originalName;

            var output = _service.GetType(_inputType,
                                            _inputDefinitions);

            _lastTokenService.Verify();
            _objectKeyStore.Verify(s => s.ThrowIfPresent(It.IsAny<string>()), Times.Never);
            _objectService.Verify(s => s.GetType(It.IsAny<OpenApiObjectType>(),
                                                It.IsAny<Dictionary<string, OpenApiObjectType>>(),
                                                It.IsAny<string>()), Times.Never);
            output.Should().Be(typeof(ExistingType));
        }

        [TestMethod]
        public void BehavesAsExpectedWhenTypeDefinedButNotCreated()
        {
            _originalName = "path/NewType";
            _processedName = "NewType";
            _inputType.Type = _originalName;

            var output = _service.GetType(_inputType,
                                            _inputDefinitions);

            _lastTokenService.Verify();
            _objectKeyStore.Verify(s => s.ThrowIfPresent(_processedName), Times.Once);
            _objectService.Verify(s => s.GetType(_definedNotCreated,
                                                _inputDefinitions,
                                                _processedName), Times.Once);
            output.Should().Be(typeof(NewType));
            CurrentTypeHolder.Types.TryGetValue(_processedName, out var fromStorage);
            fromStorage.Should().Be(typeof(NewType));
        }

        [TestMethod]
        public void ThrowsWhenNotInStorageOrDefinitions()
        {
            _processedName = "unrecognisedType";
            var expectedError = "Referenced type unrecognisedType is not " +
                "defined in OpenApi document";

            _service.Invoking(s => s.GetType(_inputType,
                                            _inputDefinitions))
                    .Should()
                    .Throw<NotSupportedException>()
                    .WithMessage(expectedError);
        }
    }
}
