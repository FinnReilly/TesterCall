using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Generation.Interface
{
    public interface IObjectsProcessingKeyStore
    {
        void ThrowIfPresent(string objectKey);
        void AddPresent(string objectKey);
        void RemovePresent(string objectKey);
    }
}
