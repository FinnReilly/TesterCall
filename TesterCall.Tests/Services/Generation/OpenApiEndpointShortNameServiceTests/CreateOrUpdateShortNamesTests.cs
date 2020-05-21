using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Enums;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Tests.Services.Generation.OpenApiEndpointShortNameServiceTests
{
    [TestClass]
    public class CreateOrUpdateShortNamesTests
    {
        private Mock<ILastTokenInPathService> _lastTokenService;

        private OpenApiEndpointShortNameService _service;

        private string _shortName;
        private string _firstTag;
        private string _secondTag;
        private string _path;
        private string _pathLastToken;

        private List<OpenApiEndpointModel> _endpoints;

        [TestInitialize]
        public void TestInitialise()
        {
            _lastTokenService = new Mock<ILastTokenInPathService>();

            _service = new OpenApiEndpointShortNameService(_lastTokenService.Object);

            _shortName = Guid.NewGuid().ToString();
            _firstTag = Guid.NewGuid().ToString();
            _secondTag = Guid.NewGuid().ToString();
            _path = Guid.NewGuid().ToString();
            _pathLastToken = Guid.NewGuid().ToString();

            _endpoints = new List<OpenApiEndpointModel>()
            {
                new OpenApiEndpointModel()
                {
                    Path = _path,
                    Tags = new List<string>()
                    {
                        _firstTag,
                        _secondTag
                    },
                    ShortName = _shortName
                },
                new OpenApiEndpointModel()
                {
                    Path = _path,
                    Tags = new List<string>()
                    {
                        _firstTag
                    },
                    Method = Method.GET
                }
            };

            _lastTokenService.Setup(s => s.GetLastToken(_path))
                .Returns(() => _pathLastToken).Verifiable();
        }

        [TestMethod]
        public void UpdatesAndCreatesNamesAsExpected()
        {
            var expectedCreatedName = $"{_firstTag}GET{_pathLastToken}";
            var expectedUpdatedName = $"{_firstTag}{_shortName}";

            _service.CreateOrUpdateShortNames(_endpoints);

            _endpoints[0].ShortName.Should().Be(expectedUpdatedName);
            _endpoints[1].ShortName.Should().Be(expectedCreatedName);
        }

        [TestMethod]
        public void CreatesAsExpectedWithPathParams()
        {
            _pathLastToken = "{id}";
            var expectedCreatedName = $"{_firstTag}GET_id_";

            _service.CreateOrUpdateShortNames(_endpoints);

            _endpoints[1].ShortName.Should().Be(expectedCreatedName);
        }
    }
}
