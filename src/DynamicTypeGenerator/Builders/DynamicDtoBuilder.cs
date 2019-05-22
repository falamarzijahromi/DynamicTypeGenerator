using DynamicTypeGenerator.Abstracts;
using DynamicTypeGenerator.Builders.Auxiliaries;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicTypeGenerator.Builders
{
    internal class DynamicDtoBuilder : DynamicTypeBuilder
    {
        public DynamicDtoBuilder(string classFullName)
        {
            var moduleBuilder = CreateModuleBuilder();

            TypeBuilder = moduleBuilder.DefineType(classFullName, TypeAttributes.Public | TypeAttributes.Class);
        }

        protected override TypeBuilder TypeBuilder { get; }

        public override IDynamicMethodBuilder SetMethod(string methodName)
        {
            throw new NotSupportedException("Method For Dto Still Doesn't Requested");
        }

        public override IDynamicPropertyBuilder SetProperty(string propertyName, Type propertyType)
        {
            var propBuilder = new DynamicDtoPropertyBuilder(propertyName, propertyType);

            AddBuildStep(propBuilder);

            return propBuilder;
        }
    }
}
