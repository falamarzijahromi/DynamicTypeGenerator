using System;
using System.Collections.Generic;
using DynamicTypeGenerator.Abstracts;
using DynamicTypeGenerator.Builders;

namespace DynamicTypeGenerator
{
    public static class DynamicTypeBuilderFactory
    {
        public static IDynamicTypeBuilder CreateDtoBuilder(string classFullName)
        {
            return new DynamicDtoBuilder(classFullName);
        }

        public static IDynamicTypeBuilder CreateInterfaceBuilder(string interfaceFullName)
        {
            return new DynamicInterfaceBuilder(interfaceFullName);
        }

        public static IDynamicTypeBuilder CreateClassBuilder(string className, Type @interface, IDictionary<string, Type> ctorParamTypeMapping)
        {
            return new DynamicClassBuilder(className, ctorParamTypeMapping, @interface);
        }

        public static IDynamicTypeBuilder CreateClassBuilder(string className, IDictionary<string, Type> ctorParamTypeMapping)
        {
            return new DynamicClassBuilder(className, ctorParamTypeMapping, null);
        }
    }
}
