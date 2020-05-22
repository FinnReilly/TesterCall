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
                            object replaceFields = null,
                            bool isExample = false)
        {
            if (IsEnumerable(type))
            {
                return CreateEnumerable(type,
                                        isExample,
                                        replaceFields);
            }

            var createdInstance = Activator.CreateInstance(type);
            var fields = createdInstance.GetType().GetFields();
            var replacementFields = replaceFields != null && replaceFields.GetType() == typeof(Hashtable) ?
                                    (Hashtable)replaceFields :
                                    null;
            var replacementsDict = replacementFields?.AsStringObjectDictionary();

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
                    !IsEnumerable(field.FieldType) &&
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
                    var createdEnumerable = CreateEnumerable(field.FieldType,
                                                            isExample,
                                                            replacementValue);

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

        public object CreateEnumerable(Type enumerableType,
                                        bool isExample,
                                        object replacementValue)
        {
            var elementType = EnumerableMemberType(enumerableType);
            
            var genericEnumerableType = typeof(List<>).MakeGenericType(elementType);
            var createdEnumerable = Activator.CreateInstance(genericEnumerableType);
            var addMethod = genericEnumerableType.GetMethod("Add",
                                                            BindingFlags.Public | BindingFlags.Instance);


            var elementTypeIsEnumerable = IsEnumerable(elementType);
            var elementTypeIsClass = elementType.IsClass 
                                    && elementType != typeof(string)
                                    && !elementTypeIsEnumerable;
            var shouldReplaceValue = replacementValue != null;
            if (!shouldReplaceValue
                && isExample)
            {
                var underlyingType = Nullable.GetUnderlyingType(elementType);

                object singleMember;
                if (elementTypeIsClass)
                {
                    singleMember = Create(elementType,
                                            null,
                                            isExample);
                }
                else if (elementTypeIsEnumerable) 
                {
                    singleMember = CreateEnumerable(elementType,
                                                    isExample,
                                                    null);
                }
                else
                {
                    singleMember = underlyingType == null ?
                                    Activator.CreateInstance(elementType) :
                                    Activator.CreateInstance(underlyingType);
                }                  

                addMethod.Invoke(createdEnumerable,
                                new object[] { singleMember });
            }
            else if (shouldReplaceValue)
            {
                // where replacement values are used:
                if (!replacementValue.GetType().IsArray)
                {
                    throw new ArgumentException("Replacement values for an array must be an array");
                }

                if (elementTypeIsClass)
                {
                    var debugType = replacementValue.GetType();

                    foreach (var hashTable in (object[])replacementValue)
                    {
                        var memberToAdd = Create(elementType,
                                                hashTable,
                                                isExample);
                        addMethod.Invoke(createdEnumerable,
                                        new object[] { memberToAdd });
                    }
                }
                else if (elementTypeIsEnumerable)
                {
                    if (!replacementValue.GetType().GetElementType().IsArray)
                    {
                        throw new ArgumentException("Replacement values in an array of arrays should " +
                            "be submitted as an array of arrays");
                    }

                    foreach (var enumerable in (object[])replacementValue)
                    {
                        var memberToAdd = CreateEnumerable(elementType,
                                                            isExample,
                                                            enumerable);
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

            return createdEnumerable;
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
