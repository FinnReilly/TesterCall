using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.Generation.Interface;

namespace TesterCall.Services.Generation
{
    public class ObjectsProcessingKeyStore : IObjectsProcessingKeyStore
    {
        private List<string> _inProcessing;

        public void AddPresent(string objectKey)
        {
            ThrowIfPresent(objectKey);
            _inProcessing.Add(objectKey);
        }

        public void RemovePresent(string objectKey)
        {
            _inProcessing.Remove(objectKey);
        }

        public void ThrowIfPresent(string objectKey)
        {
            if (_inProcessing.Contains(objectKey))
            {
                throw new InvalidOperationException($"The object type {objectKey}" +
                    $"was added to the current object processing list twice - " +
                    $"this may be due to a circular reference");
            }
        }
    }
}
