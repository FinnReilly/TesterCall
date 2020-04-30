using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.Usage.Interfaces;

namespace TesterCall.Services.Usage
{
    public class UpdateInstanceFieldsService : IUpdateInstanceFieldsService
    {
        public void UpdateFields<TInstance>(TInstance instance, 
                                            IDictionary<string, object> fieldReplacements)
        {
            var instanceProps = instance.GetType().GetProperties();

            foreach (var replacement in fieldReplacements)
            {
            }
        }
    }
}
