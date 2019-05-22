using System;
using System.Collections.Generic;
using DynamicTypeGenerator.Abstracts;

namespace DynamicTypeGenerator.Builders.Auxiliaries
{
    public class DynamicInterfacePropertyBuilder : IDynamicPropertyBuilder
    {
        public void SetAttribute(
            Type attributeType, 
            IDictionary<Type, object> ctorParametersValuesMapping, 
            IDictionary<string, object> propertiesNamesValuesMapping)
        {
            throw new NotImplementedException();
        }
    }
}
