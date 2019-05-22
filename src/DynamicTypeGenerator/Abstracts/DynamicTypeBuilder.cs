using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DynamicTypeGenerator.Builders.Auxiliaries;

namespace DynamicTypeGenerator.Abstracts
{
    internal abstract class DynamicTypeBuilder : IDynamicTypeBuilder
    {
        private readonly IList<IBuildStep> _buildSteps;

        protected DynamicTypeBuilder()
        {
            _buildSteps = new List<IBuildStep>();
        }

        public IDynamicTypeBuilder SetAttribute(Type attributeType, IDictionary<Type, object> ctorParamValueMapping, IDictionary<string, object> propertyValueMapping)
        {
            var attributeSetter =
                new DynamicTypeAttributeSetter(attributeType, ctorParamValueMapping, propertyValueMapping);

            AddBuildStep(attributeSetter);

            return this;
        }

        public abstract IDynamicPropertyBuilder SetProperty(string propertyName, Type propertyType);

        public abstract IDynamicMethodBuilder SetMethod(string methodName);

        public Type Build()
        {
            foreach (var buildStep in _buildSteps)
            {
                buildStep.Build(TypeBuilder);
            }

            return TypeBuilder.CreateType();
        }

        protected abstract TypeBuilder TypeBuilder { get; }

        protected ModuleBuilder CreateModuleBuilder()
        {
            var assemblyName = new AssemblyName("SomeAssemblyName");

            var assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            return assemblyBuilder.DefineDynamicModule("SomeModuleName");
        }

        protected void AddBuildStep(IBuildStep buildStep)
        {
            _buildSteps.Add(buildStep);
        }

    }
}
