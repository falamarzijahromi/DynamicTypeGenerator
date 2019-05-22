using System;
using System.Reflection;
using System.Reflection.Emit;
using DynamicTypeGenerator.Abstracts;
using DynamicTypeGenerator.Builders.Auxiliaries;

namespace DynamicTypeGenerator.Builders
{
    internal class DynamicInterfaceBuilder : DynamicTypeBuilder
    {
        private readonly string interfaceFullName;

        internal DynamicInterfaceBuilder(string interfaceFullName)
        {
            this.interfaceFullName = interfaceFullName;

            var moduleBuilder = CreateModuleBuilder();

            TypeBuilder = moduleBuilder.DefineType(interfaceFullName, TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);
        }

        protected override TypeBuilder TypeBuilder { get; }

        public override IDynamicMethodBuilder SetMethod(string methodName)
        {
            var methodBuilder = new DynamicInterfaceMethodBuilder(methodName);

            AddBuildStep(methodBuilder);

            return methodBuilder;
        }

        public override IDynamicPropertyBuilder SetProperty(string propertyName, Type propertyType)
        {
            throw new NotSupportedException("Property For Interface Still Doesn't Requested");
        }
    }
}
