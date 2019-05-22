using System;
using System.Collections.Generic;

namespace DynamicTypeGenerator.Abstracts
{
    public interface IDynamicAttributeSetter
    {
        void SetAttribute(
            Type attributeType,
            IDictionary<Type, object> ctorParamValueMapping,
            IDictionary<string, object> propertyValueMapping);
    }
}