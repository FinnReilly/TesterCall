﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Models.OpenApi.Interfaces;

namespace TesterCall.Services.Generation.Interface
{
    public interface IStealFieldsFromOpenApiObjectTypesService
    {
        void AddFields(IOpenApiObjectToTypeService openApiObjectToTypeService,
                        IObjectsProcessingKeyStore keyStore,
                        TypeBuilder typeBuilder,
                        IEnumerable<IOpenApiType> extendedTypes,
                        IDictionary<string, IOpenApiType> definitions);
    }
}
