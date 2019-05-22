using DynamicTypeGenerator.Abstracts;
using DynamicTypeGenerator.Builders.Auxiliaries;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicTypeGenerator.Builders
{
    internal class DynamicClassBuilder : DynamicTypeBuilder
    {
        private readonly IDictionary<string, Type> ctorParams;

        public DynamicClassBuilder(string className, IDictionary<string, Type> ctorParams)
        {
            var moduleBuilder = CreateModuleBuilder();

            TypeBuilder = moduleBuilder.DefineType(className, TypeAttributes.Class | TypeAttributes.Public);

            AddCtorParamsStep(ctorParams);
            this.ctorParams = ctorParams;
        }

        protected override TypeBuilder TypeBuilder { get; }

        public override IDynamicMethodBuilder SetMethod(string methodName)
        {
            var methodBuilder = new DynamicClassMethodBuilder(methodName, ctorParams);

            AddBuildStep(methodBuilder);

            return methodBuilder;
        }

        public override IDynamicPropertyBuilder SetProperty(string propertyName, Type propertyType)
        {
            throw new NotSupportedException("Property For Class Still Doesn't Requested");
        }

        private void AddCtorParamsStep(IDictionary<string, Type> ctorParams)
        {
            var step = new DynamicClassCtorBuilder(ctorParams);

            AddBuildStep(step);
        }
    }
}
