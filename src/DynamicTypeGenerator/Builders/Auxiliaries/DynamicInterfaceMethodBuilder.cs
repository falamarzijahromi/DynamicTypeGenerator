using DynamicTypeGenerator.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicTypeGenerator.Builders.Auxiliaries
{
    public class DynamicInterfaceMethodBuilder : IDynamicMethodBuilder, IBuildStep
    {
        private readonly string methodName;
        private readonly IList<Type> parameterTypes;
        private readonly IList<CustomAttributeBuilder> attributes;
        
        private Type returnType;

        public DynamicInterfaceMethodBuilder(string methodName)
        {
            this.methodName = methodName;

            returnType = typeof(void);

            parameterTypes = new List<Type>();

            attributes = new List<CustomAttributeBuilder>();
        }

        public void SetAttribute(
            Type attributeType, 
            IDictionary<Type, object> ctorParamValueMapping, 
            IDictionary<string, object> propertyValueMapping)
        {
            var attribute =
                DynamicAttributeFactory.CreateAttribute(attributeType, ctorParamValueMapping, propertyValueMapping);

            attributes.Add(attribute);
        }

        public IDynamicMethodBuilder SetParameter(Type parameterType)
        {
            parameterTypes.Add(parameterType);

            return this;
        }

        public IDynamicMethodBuilder SetReturnType(Type returnType)
        {
            this.returnType = returnType;

            return this;
        }

        void IBuildStep.Build(TypeBuilder typeBuilder)
        {
            var methodBuilder = typeBuilder.DefineMethod(
                methodName,
                MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual,
                CallingConventions.Standard,
                returnType,
                parameterTypes.ToArray());

            AddAttributes(methodBuilder);
        }

        private void AddAttributes(MethodBuilder methodBuilder)
        {
            foreach (var attribute in attributes)
            {
                methodBuilder.SetCustomAttribute(attribute);
            }
        }
    }
}