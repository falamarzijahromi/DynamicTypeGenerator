using System;
using System.Collections.Generic;

namespace DynamicTypeGenerator.Abstracts
{
    public interface IDynamicTypeBuilder
    {
        IDynamicTypeBuilder SetAttribute(
            Type attributeType, 
            IDictionary<Type, object> ctorParamValueMapping,
            IDictionary<string, object> propertyValueMapping);

        IDynamicPropertyBuilder SetProperty(string propertyName, Type propertyType);

        IDynamicMethodBuilder SetMethod(string methodName);

        Type Build();
    }
}
