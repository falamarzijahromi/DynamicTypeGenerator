using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using DynamicTypeGenerator.Abstracts;

namespace DynamicTypeGenerator.Builders.Auxiliaries
{
    internal class DynamicTypeAttributeSetter : IBuildStep
    {
        private readonly Type attributeType;
        private readonly IDictionary<Type, object> ctorParamValueMapping;
        private readonly IDictionary<string, object> propertyValueMapping;

        internal DynamicTypeAttributeSetter(
            Type attributeType, 
            IDictionary<Type, object> ctorParamValueMapping, 
            IDictionary<string, object> propertyValueMapping)
        {
            this.attributeType = attributeType;
            this.ctorParamValueMapping = ctorParamValueMapping;
            this.propertyValueMapping = propertyValueMapping;
        }

        void IBuildStep.Build(TypeBuilder typeBuilder)
        {
            var attribute =
                DynamicAttributeFactory.CreateAttribute(attributeType, ctorParamValueMapping, propertyValueMapping);

            typeBuilder.SetCustomAttribute(attribute);
        }
    }
}
