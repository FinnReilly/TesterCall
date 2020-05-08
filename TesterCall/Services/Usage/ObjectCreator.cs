using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TesterCall.Extensions;
using TesterCall.Services.Usage.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.Usage
{
    public class ObjectCreator : IObjectCreator
    {
        private readonly IEnumFromStringService _enumService;

        public ObjectCreator(IEnumFromStringService enumFromStringService)
        {
            _enumService = enumFromStringService;
        }

        public object Create(Type type, 
                            Hashtable replaceFields = null,
                            bool isExample = false)
        {
            var createdInstance = Activator.CreateInstance(type);
            var fields = createdInstance.GetType().GetFields();
            var replacementsDict = replaceFields?.AsStringObjectDictionary();

            //nb could make better use of dictionary here
            foreach (var field in fields)
            {
                object replacementValue = null;
                var shouldReplaceValue = false;
                if (replacementsDict != null)
                {
                    if(replacementsDict.TryGetValue(field.Name, out replacementValue))
                    {
                        shouldReplaceValue = true;
                    }
                }
                var shouldCreateValue = isExample || shouldReplaceValue; 

                if (shouldCreateValue &&
                    field.FieldType.IsClass && 
                    field.FieldType != typeof(string))
                {
                    field.SetValue(createdInstance, 
                                    Create(field.FieldType,
                                            (Hashtable)replacementValue,
                                            isExample));
                }

                //IEnumerables
                if (shouldCreateValue
                    && IsEnumerable(field.FieldType))
                {
                    var elementType = EnumerableMemberType(field.FieldType);
                    var elementIsClass = elementType.IsClass && elementType != typeof(string);
                    var enumerableType = typeof(List<>).MakeGenericType(elementType);
                    var createdEnumerable = Activator.CreateInstance(enumerableType);
                    var addMethod = enumerableType.GetMethod("Add", 
                                                            BindingFlags.Public | BindingFlags.Instance);

                    if (!shouldReplaceValue)
                    {
                        var underlyingType = Nullable.GetUnderlyingType(elementType);

                        var singleMember = elementIsClass ? 
                                                Create(elementType,
                                                        null,
                                                        isExample) : 
                                                underlyingType == null ? 
                                                    Activator.CreateInstance(elementType) :
                                                    Activator.CreateInstance(underlyingType);

                        addMethod.Invoke(createdEnumerable,
                                        new object[] { singleMember });
                    }
                    else
                    {
                        if (!replacementValue.GetType().IsArray)
                        {
                            throw new ArgumentException("Replacement values for an array must be an array");
                        }

                        if (elementIsClass)
                        {
                            if (replacementValue.GetType() != typeof(Hashtable[]))
                            {
                                throw new ArgumentException("Replacement values in an array of objects should " +
                                    "be submitted as an array of Hashtables");
                            }

                            foreach (var hashTable in (Hashtable[])replacementValue)
                            {
                                var memberToAdd = Create(elementType,
                                                        hashTable,
                                                        isExample);
                                addMethod.Invoke(createdEnumerable,
                                                new object[] { memberToAdd });
                            }
                        }
                        else
                        {
                            //handle replacement value types/strings
                            foreach (var value in (Array)replacementValue)
                            {
                                addMethod.Invoke(createdEnumerable,
                                                new object[] { value });
                            }
                        }
                    }

                    field.SetValue(createdInstance, createdEnumerable);
                }

                //value types
                if (field.FieldType.IsValueType && shouldCreateValue)
                {
                    var underlyingType = Nullable.GetUnderlyingType(field.FieldType);

                    if (underlyingType != null 
                        && !shouldReplaceValue
                        && isExample)
                    {
                        // create value for nullable types in example mode when there is no replacement
                        field.SetValue(createdInstance, Activator.CreateInstance(underlyingType));
                    }
                    else
                    {
                        if (shouldReplaceValue)
                        {
                            if (field.FieldType.IsEnum || 
                                (underlyingType != null && underlyingType.IsEnum))
                            {
                                var enumType = underlyingType != null ? underlyingType : field.FieldType;
                                replacementValue = ReturnEnum(replacementValue, enumType);
                            }

                            if (field.FieldType == typeof(DateTime) ||
                                (underlyingType != null && underlyingType == typeof(DateTime)))
                            {
                                replacementValue = ReturnDateTime(replacementValue);
                            }

                            field.SetValue(createdInstance, replacementValue);
                        }
                    }
                }

                //strings
                if (field.FieldType == typeof(string) && replacementValue != null)
                {
                    field.SetValue(createdInstance, replacementValue);
                }
            }

            return createdInstance;
        }

        private bool IsEnumerable(Type typeToCheck)
        {
            var interfaces = typeToCheck.GetInterfaces();
            return typeToCheck.IsGenericType
                    && typeToCheck.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    && typeToCheck.GenericTypeArguments.Length == 1;
        }

        private Type EnumerableMemberType(Type enumerableType)
        {
            return enumerableType.GenericTypeArguments.FirstOrDefault();
        }

        private object ReturnEnum(object suggestedReplacement, Type enumType)
        {
            if (suggestedReplacement.GetType() == typeof(string))
            {
                var makeEnumMethod = _enumService.GetType()
                                                .GetMethod("ConvertStringTo")
                                                .MakeGenericMethod(new Type[] { enumType });

                return makeEnumMethod.Invoke(_enumService,
                                            new object[] { suggestedReplacement });
            }

            return suggestedReplacement;
        }

        private object ReturnDateTime(object suggestedReplacement)
        {
            if (suggestedReplacement.GetType() == typeof(string))
            {
                return DateTime.Parse((string)suggestedReplacement);
            }

            return suggestedReplacement;
        }
    }
}
