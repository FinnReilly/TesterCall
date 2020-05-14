using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TesterCall.Enums;

namespace TesterCall.Extensions
{
    public static class MethodExtensions
    {
        public static HttpMethod ToHttpMethod(this Method method)
        {
            switch (method)
            {
                case Method.DELETE:
                    return HttpMethod.Delete;
                case Method.PATCH:
                    return new HttpMethod("Patch");
                case Method.POST:
                    return HttpMethod.Post;
                case Method.PUT:
                    return HttpMethod.Put;
                case Method.GET:
                default:
                    return HttpMethod.Get;
            }
        }
    }
}
