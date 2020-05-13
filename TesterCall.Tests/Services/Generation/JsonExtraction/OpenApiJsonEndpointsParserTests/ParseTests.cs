using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterCall.Enums;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation.JsonExtraction;
using TesterCall.Services.Generation.JsonExtraction.Interfaces;
using TesterCall.Services.Generation.JsonExtraction.Models;
using TesterCall.Services.Generation.JsonExtraction.Models.Enums;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Tests.Services.Generation.JsonExtraction.OpenApiJsonEndpointsParserTests
{
    [TestClass]
    public class ParseTests
    {
        private Mock<IOpenApiSpecUmbrellaTypeParser<JsonCatchAllTypeModel>> _typeParser;
        private Mock<IOpenApiSpecObjectParser<JsonCatchAllTypeModel>> _objectParser;
        private Mock<IEnumFromStringService> _enumService;

        private OpenApiJsonEndpointsParser _service;

        private IDictionary<string, IDictionary<string, JsonEndpointModel>> _paths;

        private JsonEndpointModel _postModelNoParams;
        private JsonEndpointModel _getModel;
        private JsonEndpointModel _postModel;

        [TestInitialize]
        public void TestInitialise()
        {
            _typeParser = new Mock<IOpenApiSpecUmbrellaTypeParser<JsonCatchAllTypeModel>>();
            _objectParser = new Mock<IOpenApiSpecObjectParser<JsonCatchAllTypeModel>>();
            _enumService = new Mock<IEnumFromStringService>();

            _service = new OpenApiJsonEndpointsParser(_typeParser.Object,
                                                        _objectParser.Object,
                                                        _enumService.Object);

            _getModel = new JsonEndpointModel()
            {
                OperationId = "GetId",
                Tags = new List<string>()
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString()
                },
                Produces = new List<string>()
                {
                    "application/json",
                    "application/text"
                },
                Responses = new Dictionary<string, JsonResponseModel>()
                {
                    { 
                        "200", 
                        new JsonResponseModel()
                        {
                            Schema = new JsonCatchAllTypeModel()
                        } 
                    }
                },
                Parameters = new List<JsonParameterModel>()
                {
                    new JsonParameterModel()
                    {
                        In = JsonParameterIn.path,
                        Required = false,
                        Type = "string"
                    },
                    new JsonParameterModel()
                    {
                        In = JsonParameterIn.query,
                        Required = true,
                        Type = "integer",
                        Format = "int64"
                    }
                }
            };
            _postModelNoParams = new JsonEndpointModel()
            {
                OperationId = "PostId",
                Tags = new List<string>()
                {
                    Guid.NewGuid().ToString()
                },
                Produces = new List<string>()
                {
                    "application/json",
                    "application/text"
                },
                Responses = new Dictionary<string, JsonResponseModel>()
                {
                    {
                        "204",
                        new JsonResponseModel()
                        {
                            Schema = new JsonCatchAllTypeModel()
                        }
                    }
                }
            };
            _postModel = new JsonEndpointModel()
            {
                OperationId = "PostId",
                Tags = new List<string>()
                {
                    Guid.NewGuid().ToString()
                },
                Produces = new List<string>()
                {
                    "application/json",
                    "application/text"
                },
                Consumes = new List<string>()
                {
                   "application/json",
                   "application/text"
                },
                Parameters = new List<JsonParameterModel>()
                {
                    new JsonParameterModel()
                    {
                        In = JsonParameterIn.body
                    },
                    new JsonParameterModel()
                    {
                        In = JsonParameterIn.path,
                        Type = "string"
                    }
                },
                Responses = new Dictionary<string, JsonResponseModel>()
                {
                    {
                        "204",
                        new JsonResponseModel()
                        {
                            Schema = new JsonCatchAllTypeModel()
                        }
                    }
                }
            };

            _paths = new Dictionary<string, IDictionary<string, JsonEndpointModel>>()
            {
                {
                    "testPath",
                    new Dictionary<string, JsonEndpointModel>()
                    {
                        { "get", _getModel },
                        { "post", _postModel }
                    }
                },
                { 
                    "testNoParamsPath",
                    new Dictionary<string, JsonEndpointModel>()
                    {
                        { "post", _postModelNoParams }
                    }
                }
            };

            _enumService.Setup(s => s.ConvertStringTo<Method>("GET"))
                .Returns(Method.GET);
            _enumService.Setup(s => s.ConvertStringTo<Method>("POST"))
                .Returns(Method.POST);
            _enumService.Setup(s => s.ConvertStringTo<ParameterIn>("path"))
                .Returns(ParameterIn.path);
            _enumService.Setup(s => s.ConvertStringTo<ParameterIn>("query"))
                .Returns(ParameterIn.query);
            _typeParser.Setup(s => s.Parse(_objectParser.Object,
                                            It.IsAny<JsonCatchAllTypeModel>()))
                .Returns(() => new OpenApiObjectType());
        }

        [TestMethod]
        public void BehavesAsExpected()
        {
            var output = _service.Parse(_paths);

            output.Count().Should().Be(3);
            var noParams = output.Where(e => e.Path == "testNoParamsPath");
            var hasParams = output.Where(e => e.Path == "testPath");
            noParams.Count().Should().Be(1);
            hasParams.Count().Should().Be(2);
            var noParamEndpoint = noParams.First();
            var paramsPost = hasParams.First(e => e.Method == Method.POST);
            var paramsGet = hasParams.First(e => e.Method == Method.GET);
            noParamEndpoint.Parameters.Should().BeNull();
            paramsPost.Parameters.Count().Should().Be(1);
            paramsGet.Parameters.Count().Should().Be(2);
            paramsGet.Parameters.Count(p => p.Required
                                        && p.In == ParameterIn.query).Should().Be(1);
            paramsGet.Parameters.Count(p => !p.Required
                                        && p.In == ParameterIn.path).Should().Be(1);
            paramsPost.RequestBody.Content.
                                    GetType()
                                    .Should().Be(typeof(OpenApiObjectType));
            paramsPost.SuccessStatusResponse.Content
                                            .GetType()
                                            .Should().Be(typeof(OpenApiObjectType));
            paramsGet.SuccessStatusResponse.Content
                                            .GetType()
                                            .Should().Be(typeof(OpenApiObjectType));
            noParamEndpoint.SuccessStatusResponse.Content
                                                .GetType()
                                                .Should().Be(typeof(OpenApiObjectType));
        }
    }
}
