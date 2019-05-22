using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace DynamicTypeGenerator.Builders.Auxiliaries
{
    internal class DynamicAttributeFactory
    {
        public static CustomAttributeBuilder CreateAttribute(
            Type attributeType,
            IDictionary<Type, object> ctoParamValueMapping,
            IDictionary<string, object> propertyValueMapping)
        {
                var attributeCtor = attributeType.GetConstructor(ctoParamValueMapping.Keys.ToArray());

                var propertyInfos = propertyValueMapping.Keys.Select(propName => attributeType.GetProperty(propName));

                var attribute = new CustomAttributeBuilder(
                    attributeCtor,
                    ctoParamValueMapping.Values.ToArray(),
                    propertyInfos.ToArray(),
                    propertyValueMapping.Values.ToArray());

                return attribute;
        }
    }
}
