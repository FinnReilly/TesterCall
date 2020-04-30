using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterCall.Services.Usage.Interfaces;

namespace TesterCall.Services.Usage
{
    public class ObjectCreator : IObjectCreator
    {
        public object Create(Type type, IDictionary<string, object> replaceFields = null)
        {
            var createdInstance = Activator.CreateInstance(type);
            var fields = createdInstance.GetType().GetFields();

            foreach (var field in fields)
            {
                if (field.FieldType.IsClass)
                {
                    field.SetValue(createdInstance, Create(field.FieldType));
                }

                if (field.FieldType.IsArray)
                {
                    var elementType = field.FieldType.GetElementType();
                    var array = Array.CreateInstance(elementType, 1);
                    if (field.FieldType.GetElementType().IsClass)
                    {
                        var singleMember = Create(elementType);
                        array.SetValue(singleMember, 0);
                    }

                    field.SetValue(createdInstance, array);
                }
            }

            if (replaceFields != null)
            {

                foreach (var replacement in replaceFields)
                {
                    var isNestedReplacement = replacement.Value != null
                                                && replacement.Value
                                                                .GetType()
                                                                .IsAssignableFrom(typeof(IDictionary<,>));
                    var fieldToReplace = fields.FirstOrDefault(p => p.Name == replacement.Key);

                    if (fieldToReplace == null)
                    {
                        throw new ArgumentException($"Type {type.Name} does not have a property named " +
                            $"{replacement.Key}");
                    }

                    if (!isNestedReplacement)
                    {
                        if (replacement.Value != null &&
                            fieldToReplace.FieldType != replacement.Value.GetType())
                        {
                            throw new ArgumentException($"Property {fieldToReplace.Name} of object type " +
                                $"{type.Name} should be of type {fieldToReplace.FieldType} but type " +
                                $"provided was {replacement.Value.GetType()}");
                        }

                        fieldToReplace.SetValue(createdInstance, replacement.Value);
                    }
                    else
                    {
                        if (!fieldToReplace.FieldType.IsClass)
                        {
                            throw new ArgumentException($"Property {fieldToReplace.Name} of " +
                                $"object type {type.Name} is not a nested class - " +
                                $"a dictionary cannot be used to set its value");
                        }

                        var objectValue = Create(fieldToReplace.FieldType,
                                                (IDictionary<string, object>)replacement.Value);
                        fieldToReplace.SetValue(createdInstance, objectValue);
                    }
                }
            }

            return createdInstance;
        }
    }
}
