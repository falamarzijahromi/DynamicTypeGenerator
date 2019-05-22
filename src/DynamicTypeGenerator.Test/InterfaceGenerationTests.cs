using DynamicTypeGenerator.Abstracts;
using DynamicTypeGenerator.Tests.SampleTypes;
using System;
using System.Collections.Generic;
using Xunit;

namespace DynamicTypeGenerator.Tests
{
    public class InterfaceGenerationTests
    {
        [Fact]
        public void Dynamic_Interface_Must_Be_Generated_With_Its_Methods()
        {

            var method1 = "Method1";
            var method1ReturnType = typeof(string);
            var method1Params = new[] { typeof(int), typeof(string), typeof(Guid) };

            var method2 = "Method2";
            var method2ReturnType = typeof(void);
            var method2Params = new[] { typeof(object), typeof(Guid), typeof(long) };

            var interfaceFullName = "Dynamic.GeneratedInterface";

            var interfaceBuilder = DynamicTypeBuilderFactory.CreateInterfaceBuilder(interfaceFullName);

            DefineMethod(method1, method1ReturnType, method1Params, interfaceBuilder);
            DefineMethod(method2, method2ReturnType, method2Params, interfaceBuilder);

            var interfaceType = interfaceBuilder.Build();

            AssertOnHavingMethod(
                type: interfaceType,
                methodName: method1,
                returnType: method1ReturnType,
                paramTypes: method1Params);

            AssertOnHavingMethod(
                type: interfaceType,
                methodName: method2,
                returnType: method2ReturnType,
                paramTypes: method2Params);
        }

        [Fact]
        public void Dynamic_Interface_Must_Be_Generated_With_Defined_Attribute_On_The_Defined_Method()
        {
            var method1 = "Method1";

            var interfaceFullName = "Dynamic.GeneratedInterface";
            var attributeType = typeof(SampleAttribute);
            var attributeCtorParamValueMapping = SampleAttribute.GetCtorParamValueMapping();
            var attributePropertyValueMapping = SampleAttribute.GetPropertyValueMaaping();

            var interfaceBuilder = DynamicTypeBuilderFactory.CreateInterfaceBuilder(interfaceFullName);

            interfaceBuilder
                .SetMethod(method1)
                .SetAttribute(attributeType, attributeCtorParamValueMapping, attributePropertyValueMapping);

            var generatedType = interfaceBuilder.Build();

            AssertOnHavingAttributeOnMethod(
                interfaceType: generatedType,
                methodName: method1,
                attributeType: attributeType,
                attributeCtorParamValueMapping: attributeCtorParamValueMapping,
                attributePropertyValueMapping: attributePropertyValueMapping);
        }

        [Fact]
        public void Dynamic_Interface_Must_Be_Generated_With_Defined_Attribute()
        {
            var interfaceFullName = "Dynamic.GeneratedInterface";
            var attributeType = typeof(SampleAttribute);
            var attributeCtorParamValueMapping = SampleAttribute.GetCtorParamValueMapping();
            var attributePropertyValueMapping = SampleAttribute.GetPropertyValueMaaping();

            var interfaceBuilder = DynamicTypeBuilderFactory.CreateInterfaceBuilder(interfaceFullName);

            interfaceBuilder.SetAttribute(attributeType, attributeCtorParamValueMapping, attributePropertyValueMapping);

            var generatedType = interfaceBuilder.Build();

            AssertOnHavingAttributeOnType(
                interfaceType: generatedType,
                attributeType: attributeType,
                attributeCtorParamValueMapping: attributeCtorParamValueMapping,
                attributePropertyValueMapping: attributePropertyValueMapping);
        }

        private void AssertOnHavingAttributeOnType(Type interfaceType, Type attributeType, IDictionary<Type, object> attributeCtorParamValueMapping, IDictionary<string, object> attributePropertyValueMapping)
        {
            Assert.True(ReflectionHelper.HasAttributeOnType(interfaceType, attributeType, attributeCtorParamValueMapping, attributePropertyValueMapping));
        }

        private void AssertOnHavingAttributeOnMethod(Type interfaceType, string methodName, Type attributeType, IDictionary<Type, object> attributeCtorParamValueMapping, IDictionary<string, object> attributePropertyValueMapping)
        {
            Assert.True(ReflectionHelper.HasAttributeOnMethodWithFollowingPropertyValues(interfaceType, methodName, attributeType, attributePropertyValueMapping));
        }

        private void DefineMethod(string method1, Type method1ReturnType, Type[] method1Params, IDynamicTypeBuilder interfaceBuilder)
        {
            var methodBuilder = interfaceBuilder
                            .SetMethod(method1)
                            .SetReturnType(method1ReturnType);

            foreach (var method1Param in method1Params)
            {
                methodBuilder.SetParameter(method1Param);
            }
        }

        private void AssertOnHavingMethod(Type type, string methodName, Type returnType, Type[] paramTypes)
        {
            Assert.True(ReflectionHelper.HasMethod(type, methodName, returnType, paramTypes));
        }
    }
}