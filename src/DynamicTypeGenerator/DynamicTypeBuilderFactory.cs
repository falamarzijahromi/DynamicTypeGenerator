using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using DynamicTypeGenerator.Abstracts;
using DynamicTypeGenerator.Builders;

namespace DynamicTypeGenerator
{
    public static class DynamicTypeBuilderFactory
    {
        public static IDynamicTypeBuilder CreateDtoBuilder(string classFullName, ModuleBuilder moduleBuilder = null)
        {
            return new DynamicDtoBuilder(classFullName, moduleBuilder);
        }

        public static IDynamicTypeBuilder CreateInterfaceBuilder(string interfaceFullName, ModuleBuilder moduleBuilder = null, params Type[] interfaces)
        {
            return new DynamicInterfaceBuilder(interfaceFullName, moduleBuilder, interfaces);
        }

        public static IDynamicTypeBuilder CreateClassBuilder(string className, IDictionary<string, Type> ctorParamTypeMapping, ModuleBuilder moduleBuilder = null ,params Type[] interfaces)
        {
            return new DynamicClassBuilder(className, ctorParamTypeMapping, moduleBuilder, interfaces);
        }
    }
}
