using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DynamicTypeGenerator.Abstracts;

namespace DynamicTypeGenerator.Builders.Auxiliaries
{
    internal class DynamicClassMethodBuilder : IDynamicMethodBuilder, IBuildStep
    {
        private readonly string methodName;
        private readonly IDictionary<string, Type> injectedFields;
        private readonly IList<CustomAttributeBuilder> attributes;
        private readonly IList<Type> @params;

        private Type returnType;

        public DynamicClassMethodBuilder(string methodName, IDictionary<string, Type> injectedFields)
        {
            this.methodName = methodName;
            this.injectedFields = injectedFields;
            attributes = new List<CustomAttributeBuilder>();
            @params = new List<Type>();

            returnType = typeof(void);
        }

        public void Build(TypeBuilder typeBuilder)
        {
            var methodBuilder = DefineMethod(typeBuilder);

            DefineMethodBody(methodBuilder);
        }

        public void SetAttribute(
            Type attributeType,
            IDictionary<Type, object> ctorParametersValuesMapping,
            IDictionary<string, object> propertiesNamesValuesMapping)
        {
            var attribute = DynamicAttributeFactory.CreateAttribute(attributeType, ctorParametersValuesMapping,
                propertiesNamesValuesMapping);

            attributes.Add(attribute);
        }

        public IDynamicMethodBuilder SetParameter(Type parameterType)
        {
            @params.Add(parameterType);

            return this;
        }

        public IDynamicMethodBuilder SetReturnType(Type returnType)
        {
            this.returnType = returnType;

            return this;
        }

        private void DefineMethodBody(MethodBuilder methodBuilder)
        {
            var ilGen = methodBuilder.GetILGenerator();

            ilGen.Emit(OpCodes.Ret);
        }

        private MethodBuilder DefineMethod(TypeBuilder typeBuilder)
        {
            return typeBuilder.DefineMethod(
                methodName, MethodAttributes.Public, 
                CallingConventions.HasThis, returnType,
                @params.ToArray());
        }
    }
}