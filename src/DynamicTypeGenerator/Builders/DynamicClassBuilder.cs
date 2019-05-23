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
	    private readonly IList<FieldBuilder> fields;

        public DynamicClassBuilder(string className, IDictionary<string, Type> ctorParams)
        {
            var moduleBuilder = CreateModuleBuilder();

            TypeBuilder = moduleBuilder.DefineType(
                className, 
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit);

			var ctorBuilder = new DynamicClassCtorBuilder(ctorParams);

	        fields = ctorBuilder.Build(TypeBuilder);
        }

        protected override TypeBuilder TypeBuilder { get; }

        public override IDynamicMethodBuilder SetMethod(string methodName)
        {
            var methodBuilder = new DynamicClassMethodBuilder(methodName, fields);

            AddBuildStep(methodBuilder);

            return methodBuilder;
        }

        public override IDynamicPropertyBuilder SetProperty(string propertyName, Type propertyType)
        {
            throw new NotSupportedException("Property For Class Still Doesn't Requested");
        }
    }
}
