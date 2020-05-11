using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterCall.Models.Endpoints;
using TesterCall.Services.Usage.Interfaces;

namespace TesterCall.Services.Usage
{
    public class CheckRequiredParametersService : ICheckRequiredParametersService
    {
        public void CheckRequiredParametersPresent(Endpoint endpoint, 
                                                    IDictionary<string, string> queryParams, 
                                                    IDictionary<string, string> pathParams, 
                                                    IDictionary<string, string> headerParams)
        {
            var errorContent = new List<string>();

            var requiredQueryParams = endpoint.QueryParameters.Where(p => p.Required);
            var requiredPathParams = endpoint.PathParameters.Where(p => p.Required);
            var requiredHeaderParams = endpoint.HeaderParameters.Where(p => p.Required);
            errorContent.Add(CheckParameterGroup(requiredQueryParams,
                                                queryParams,
                                                "query"));
            errorContent.Add(CheckParameterGroup(requiredPathParams,
                                                        pathParams,
                                                        "path"));
            errorContent.Add(CheckParameterGroup(requiredHeaderParams,
                                                        headerParams,
                                                        "header"));

            if (errorContent.Any(e => !string.IsNullOrEmpty(e)))
            {
                throw new ArgumentException($"The following required parameters are missing: \n" +
                                            $"{string.Join("\n", errorContent)}");
            }
        }

        private string CheckParameterGroup(IEnumerable<Parameter> required, 
                                            IDictionary<string, string> supplied,
                                            string location)
        {
            var errorString = "";

            foreach (var parameter in required)
            {
                if (!supplied.TryGetValue(parameter.Name, out var value))
                {
                    errorString += $"{parameter.Name} in {location} \n";
                }
            }

            if (errorString.Length == 0)
            {
                return null;
            }

            return errorString;
        }
    }
}
