using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace DynamicTypeGenerator.Tests
{
    public static class ReflectionHelper
    {
        public static bool TypeFullNameIs(Type type, string typeFullName)
        {
            return type.FullName == typeFullName;
        }

        public static bool HasFullProperty(Type type, string propertyName, Type propertyType)
        {
            var propertyInfo = type.GetProperty(propertyName);

            return
                propertyInfo != null &&
                propertyInfo.PropertyType == propertyType &&
                propertyInfo.GetGetMethod() != null &&
                propertyInfo.GetSetMethod() != null;
        }

        public static bool HasWorkingProperty(Type type, string propertyName, object value)
        {
            var propertyInfo = type.GetProperty(propertyName);

            var instance = Activator.CreateInstance(type);
            var defaultValue = GetDefaultValue(value.GetType());

            var gottenDefaultValue = GetFromProperty(propertyInfo, instance);

            propertyInfo.GetSetMethod(false)
                .Invoke(
                    instance,
                    BindingFlags.Public,
                    null,
                    new object[] { value },
                    CultureInfo.CurrentCulture);

            var gottenValue = GetFromProperty(propertyInfo, instance);

            return
                gottenDefaultValue == GetDefaultValue(value.GetType()) &&
                gottenValue == value;
        }

        public static bool HasAttributeOnPropertyWithFollowingPropertyValues(
            Type generatedType,
            string propertyName,
            Type attributeType,
            IDictionary<string, object> propertiesValuesMapping)
        {
            var member = generatedType.GetProperty(propertyName);

            return CheckAttributeSatisfaction(attributeType, propertiesValuesMapping, member);
        }

        public static bool HasMethod(Type type, string methodName, Type returnType, IList<Type> paramTypes)
        {
            var methodInfo = type.GetMethod(methodName);

            if (methodInfo == null || methodInfo.ReturnType != returnType)
            {
                return false;
            }

            var methodParams = methodInfo.GetParameters();

            if (methodParams.Length != paramTypes.Count)
            {
                return false;
            }

            for (int i = 0; i < paramTypes.Count; i++)
            {
                var paramIsOk = methodParams[i].ParameterType == paramTypes[i];

                if (!paramIsOk)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool HasCtorWithSpecifiedParamsType(Type classType, params Type[] ctorParams)
        {
            var classCtor = classType.GetConstructor(ctorParams);

            return (classCtor != null);
        }

        public static bool HasAttributeOnMethodWithFollowingPropertyValues(Type interfaceType, string methodName, Type attributeType, IDictionary<string, object> attributePropertyValueMapping)
        {
            var member = interfaceType.GetMethod(methodName);

            return CheckAttributeSatisfaction(attributeType, attributePropertyValueMapping, member);
        }

        public static bool InstanciatedObjectHasSpecifiedFieldValues(Type classType, Dictionary<string, object> fieldValues)
        {
            var @object = Activator.CreateInstance(classType, fieldValues.Values.ToArray());

            var allFields = classType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            if (allFields.Length != fieldValues.Count)
            {
                return false;
            }

            foreach (var field in allFields)
            {
                var fieldValue = field.GetValue(@object);

                if (!fieldValue.Equals(fieldValues[field.Name]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool HasAttributeOnType(Type interfaceType, Type attributeType, IDictionary<Type, object> attributeCtorParamValueMapping, IDictionary<string, object> attributePropertyValueMapping)
        {
            return CheckAttributeSatisfaction(attributeType, attributePropertyValueMapping, interfaceType);
        }

	    public static void ExecuteMethod(
		    Type classType, 
		    string methodName, 
		    Dictionary<Type, object> ctorParamValueMapping, 
		    Dictionary<Type, object> paramsValueMapping)
	    {
		    var @object = Activator.CreateInstance(classType, ctorParamValueMapping.Values.ToArray());

		    var method = @object.GetType().GetMethod(methodName);

		    var result = method.Invoke(@object, paramsValueMapping.Values.ToArray());
	    }

        public static bool MethodHasAttributes(Type classType, string methodName, IDictionary<Type, IDictionary<string, object>> propValuesMapping)
        {
            var method = classType.GetMethod(methodName);

            var satisfied = true;

            foreach (var mapping in propValuesMapping)
            {
                satisfied = satisfied && CheckAttributeSatisfaction(mapping.Key, mapping.Value, method);
            }

            return satisfied;
        }

        private static bool CheckAttributeSatisfaction(Type attributeType, IDictionary<string, object> attributePropertyValueMapping, MemberInfo member)
        {
            var attributeData = GetAttribute(member, attributeType);

            if (attributeData == null)
            {
                return false;
            }

            bool allPropertiesSatisfied = CheckPropertiesOfAttributes(attributePropertyValueMapping, attributeData);

            return allPropertiesSatisfied;
        }

        private static CustomAttributeData GetAttribute(MemberInfo memberInfo, Type attributeType)
        {
            return memberInfo.CustomAttributes
                .SingleOrDefault(cAttr => attributeType.IsAssignableFrom(cAttr.AttributeType));
        }

        private static bool CheckPropertiesOfAttributes(IDictionary<string, object> propertiesValuesMapping, CustomAttributeData attributeData)
        {
            return propertiesValuesMapping.All(propVal =>
                attributeData.NamedArguments.Any(arg =>
                    arg.MemberName.Equals(propVal.Key) && arg.TypedValue.Value.Equals(propVal.Value)));
        }

        private static object GetFromProperty(PropertyInfo propertyInfo, object instance)
        {
            return propertyInfo.GetGetMethod(false)
                .Invoke(
                    instance,
                    BindingFlags.Public,
                    null,
                    new object[] { },
                    CultureInfo.CurrentCulture);
        }

        private static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}
