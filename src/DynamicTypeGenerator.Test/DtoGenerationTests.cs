using DynamicTypeGenerator.Tests.SampleTypes;
using System;
using System.Collections.Generic;
using Xunit;

namespace DynamicTypeGenerator.Tests
{
    public class DtoGenerationTests
    {
        [Fact]
        public void Dynamic_Dto_Type_Must_Be_Created_With_All_Of_Defined_Full_Properties()
        {
            var typeName = "DynamicDto";

            var sampleStringPropertyName = "SampleStringProperty";
            var sampleStringPropertyType = typeof(string);

            var sampleIntPropertyName = "SampleIntProperty";
            var sampleIntPropertyType = typeof(int);

            var generatedType = GenerateDynamicDtoType(
                typeName,
                new Dictionary<string, Type>
            {
                {sampleIntPropertyName, sampleIntPropertyType},
                {sampleStringPropertyName, sampleStringPropertyType},
            });

            AssertTypeName(generatedType, typeName);

            AssertOnHavingProperty(
                generatedType: generatedType,
                name: sampleStringPropertyName,
                propertyType: sampleStringPropertyType);

            AssertOnHavingProperty(
                generatedType: generatedType,
                name: sampleIntPropertyName,
                propertyType: sampleIntPropertyType);
        }

        [Fact]
        public void Dynamic_Dto_Type_Must_Have_The_Defined_Working_Property()
        {
            var propertyName = "SomeProperty";
            var propertyType = typeof(string);

            var generatedType =
                GenerateDynamicDtoType(
                    "DynamicDto",
                    new Dictionary<string, Type> { { propertyName, propertyType } });

            AssertOnHavingWorkingProperty(generatedType, propertyName, "Hello");
        }

        [Fact]
        public void Dynamic_Dto_Type_Must_Have_The_Specified_Attribute_On_The_Defined_Property()
        {
            var propertyName = "SomeProperty";
            var attributeType = typeof(SampleAttribute);
            var ctorParametersValuesMapping = SampleAttribute.GetCtorParamValueMapping();
            var propertiesValuesMapping = SampleAttribute.GetPropertyValueMaaping();

            var typeBuilder = DynamicTypeBuilderFactory.CreateDtoBuilder(propertyName);

            typeBuilder
                .SetProperty(propertyName, typeof(string))
                .SetAttribute(
                    attributeType,
                    ctorParametersValuesMapping,
                    propertiesValuesMapping);

            var generatedType = typeBuilder.Build();

            AssertOnHavingPropertyWithFollowingAttribute(generatedType, propertyName, typeof(SampleAttribute), propertiesValuesMapping);
        }

        private void AssertOnHavingPropertyWithFollowingAttribute(
            Type generatedType,
            string propertyName,
            Type attributeType,
            IDictionary<string, object> propertiesValuesMapping)
        {
            Assert.True(ReflectionHelper.HasAttributeOnPropertyWithFollowingPropertyValues(generatedType, propertyName, attributeType, propertiesValuesMapping));
        }

        private void AssertOnHavingWorkingProperty(
            Type generatedType,
            string propertyName,
            object propertyValue)
        {
            Assert.True(ReflectionHelper.HasWorkingProperty(generatedType, propertyName, propertyValue));
        }

        private static Type GenerateDynamicDtoType(
            string typeName,
            IDictionary<string, Type> propertiesNameTypeMapping)
        {
            var typeBuilder = DynamicTypeBuilderFactory.CreateDtoBuilder(typeName);

            foreach (var propertyName in propertiesNameTypeMapping.Keys)
            {
                typeBuilder.SetProperty(propertyName, propertiesNameTypeMapping[propertyName]);
            }

            var generatedType = typeBuilder.Build();

            return generatedType;
        }

        private void AssertTypeName(Type generatedType, string typeName)
        {
            Assert.Equal(typeName, generatedType.FullName);
        }

        private void AssertOnHavingProperty(Type generatedType, string name, Type propertyType)
        {
            Assert.True(ReflectionHelper.HasFullProperty(generatedType, name, propertyType));
        }
    }
}
