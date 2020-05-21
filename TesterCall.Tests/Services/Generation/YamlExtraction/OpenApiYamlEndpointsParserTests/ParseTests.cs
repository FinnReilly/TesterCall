using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterCall.Enums;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation.JsonExtraction.Interfaces;
using TesterCall.Services.Generation.YamlExtraction;
using TesterCall.Services.Generation.YamlExtraction.Models;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Tests.Services.Generation.YamlExtraction.OpenApiYamlEndpointsParserTests
{
    [TestClass]
    public class ParseTests
    {
        private Mock<IOpenApiSpecUmbrellaTypeParser<YamlCatchAllTypeModel>> _typeParser;
        private Mock<IOpenApiSpecObjectParser<YamlCatchAllTypeModel>> _objectParser;
        private Mock<IEnumFromStringService> _enumService;

        private OpenApiYamlEndpointsParser _service;

        private Dictionary<string, YamlPathModel> _paths;

        private YamlCatchAllTypeModel _path1GetResponseContent;
        private YamlCatchAllTypeModel _path1PostRequestContent;
        private YamlCatchAllTypeModel _path2GetResponseContent;
        private YamlCatchAllTypeModel _path1GetFailureResponseContent;

        private OpenApiPrimitiveType _parameterOpenApiType;
        private OpenApiObjectType _path1GetParsedResponse;
        private OpenApiObjectType _path1PostParsedRequest;
        private OpenApiObjectType _path2GetParsedResponse;

        [TestInitialize]
        public void TestInitialise()
        {
            _typeParser = new Mock<IOpenApiSpecUmbrellaTypeParser<YamlCatchAllTypeModel>>();
            _objectParser = new Mock<IOpenApiSpecObjectParser<YamlCatchAllTypeModel>>();
            _enumService = new Mock<IEnumFromStringService>();

            _service = new OpenApiYamlEndpointsParser(_typeParser.Object,
                                                        _objectParser.Object,
                                                        _enumService.Object);

            _path1GetResponseContent = new YamlCatchAllTypeModel();
            _path1PostRequestContent = new YamlCatchAllTypeModel();
            _path2GetResponseContent = new YamlCatchAllTypeModel();
            _path1GetFailureResponseContent = new YamlCatchAllTypeModel();

            _parameterOpenApiType = new OpenApiPrimitiveType();
            _path1GetParsedResponse = new OpenApiObjectType();
            _path2GetParsedResponse = new OpenApiObjectType();
            _path1PostParsedRequest = new OpenApiObjectType();

            _paths = new Dictionary<string, YamlPathModel>()
            {
                {
                    "/api/path1", 
                    new YamlPathModel()
                    {
                        //path scope parameters should be handled correctly
                        Parameters = new List<YamlParameterModel>()
                        {
                            new YamlParameterModel()
                            {
                                In = ParameterIn.query,
                                Name = "pathScopeQueryParam",
                                Schema = new YamlCatchAllTypeModel()
                                {
                                    Type = "string"
                                }
                            }
                        },
                        Get = new YamlEndpointModel()
                        {
                            Responses = new Dictionary<string, YamlRequestResponseModel>()
                            {
                                {
                                    "200",
                                    new YamlRequestResponseModel()
                                    {
                                        Content =  new Dictionary<string, YamlContentModel>()
                                        {
                                            {
                                                "application/json",
                                                new YamlContentModel()
                                                {
                                                    Schema = _path1GetResponseContent
                                                }
                                            }
                                        }
                                    }
                                },
                                // test failure responses ignored
                                {
                                    "400",
                                    new YamlRequestResponseModel()
                                    {
                                        Content = new Dictionary<string, YamlContentModel>()
                                        {
                                            {
                                                "application/json",
                                                new YamlContentModel()
                                                {
                                                    Schema = _path1GetFailureResponseContent
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        Post = new YamlEndpointModel()
                        {
                            RequestBody = new YamlRequestResponseModel()
                            {
                                Content = new Dictionary<string, YamlContentModel>()
                                {
                                    {
                                        "application/json",
                                        new YamlContentModel()
                                        {
                                            Schema = _path1PostRequestContent
                                        }
                                    }
                                }
                            },
                            Responses = new Dictionary<string, YamlRequestResponseModel>()
                            {
                                // test empty responses are parsed correctly
                                {
                                    "200",
                                    new YamlRequestResponseModel()
                                }
                            }
                        }
                    } 
                },
                {
                    "/api/path2",
                    new YamlPathModel()
                    {
                        Get = new YamlEndpointModel()
                        {
                            Responses = new Dictionary<string, YamlRequestResponseModel>()
                            {
                                {
                                    //test all success statuses recognised
                                    "204",
                                    new YamlRequestResponseModel()
                                    {
                                        Content = new Dictionary<string, YamlContentModel>()
                                        {
                                            {
                                                "application/json",
                                                new YamlContentModel()
                                                {
                                                    Schema = _path2GetResponseContent
                                                }
                                            }
                                        }
                                    }
                                }
                            },

                            // endpoint scope parameters should be handled correctly
                            Parameters = new List<YamlParameterModel>()
                            {
                                new YamlParameterModel()
                                {
                                    Name = "path2EndpointScopeHeaderParam",
                                    Schema = new YamlCatchAllTypeModel()
                                    {
                                        Type = "string"
                                    },
                                    In = ParameterIn.header
                                }
                            }
                        }
                    }
                }
            };

            _typeParser.Setup(s => s.Parse(_objectParser.Object,
                                            It.Is<YamlCatchAllTypeModel>(y => y.Type == "string")))
                        .Returns(_parameterOpenApiType);
            _typeParser.Setup(s => s.Parse(_objectParser.Object,
                                            _path1GetResponseContent))
                .Returns(_path1GetParsedResponse);
            _typeParser.Setup(s => s.Parse(_objectParser.Object,
                                            _path2GetResponseContent))
                .Returns(_path2GetParsedResponse);
            _typeParser.Setup(s => s.Parse(_objectParser.Object,
                                            _path1PostRequestContent))
                .Returns(_path1PostParsedRequest);
            _enumService.Setup(s => s.ConvertStringTo<Method>("GET"))
                .Returns(Method.GET);
            _enumService.Setup(s => s.ConvertStringTo<Method>("POST"))
                .Returns(Method.POST);
        }

        [TestMethod]
        public void ParsesEndpointsAsExpected()
        {
            var output = _service.Parse(_paths);

            // do not parse failure content
            _typeParser.Verify(s => s.Parse(_objectParser.Object,
                                            _path1GetFailureResponseContent), Times.Never);

            output.Count().Should().Be(3);
            // are path level params handled?
            output.Count(e => e.Path.Contains("path1")
                            && e.Parameters.Count() == 1
                            && e.Parameters.First().Schema == _parameterOpenApiType
                            && e.Parameters.First().Name == "pathScopeQueryParam"
                            && e.Parameters.First().In == ParameterIn.query).Should().Be(2);

            var get1 = output.FirstOrDefault(e => e.Path.Contains("path1") && e.Method == Method.GET);
            var get2 = output.FirstOrDefault(e => e.Path.Contains("path2") && e.Method == Method.GET);
            var post = output.FirstOrDefault(e => e.Path.Contains("path1") && e.Method == Method.POST);

            get1.Should().NotBeNull();
            get1.SuccessStatusResponse.Content.Should().Be(_path1GetParsedResponse);
            get1.SuccessStatusResponse.Type.Should().Be("application/json");
            get2.Should().NotBeNull();
            get2.SuccessStatusResponse.Content.Should().Be(_path2GetParsedResponse);
            get2.SuccessStatusResponse.Type.Should().Be("application/json");
            get2.Parameters.Count().Should().Be(1);
            get2.Parameters.First().Name.Should().Be("path2EndpointScopeHeaderParam");
            get2.Parameters.First().In.Should().Be(ParameterIn.header);
            get2.Parameters.First().Schema.Should().Be(_parameterOpenApiType);
            post.Should().NotBeNull();
            post.RequestBody.Type.Should().Be("application/json");
            post.RequestBody.Content.Should().Be(_path1PostParsedRequest);
            post.SuccessStatusResponse.Should().BeNull();
        }
    }
}
