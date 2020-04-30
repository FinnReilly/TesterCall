using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Usage.Interfaces
{
    public interface IUpdateInstanceFieldsService
    {
        void UpdateFields<TInstance>(TInstance instance, IDictionary<string, object> fieldReplacements);
    }
}
