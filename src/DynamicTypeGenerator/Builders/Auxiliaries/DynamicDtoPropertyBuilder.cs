using DynamicTypeGenerator.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicTypeGenerator.Builders.Auxiliaries
{
    public class DynamicDtoPropertyBuilder : IDynamicPropertyBuilder, IBuildStep
    {
        private readonly string _propertyName;
        private readonly Type _propertyType;
        private readonly IList<CustomAttributeBuilder> _attributes;

        internal DynamicDtoPropertyBuilder(
            string propertyName,
            Type propertyType)
        {
            _propertyName = propertyName;
            _propertyType = propertyType;
            _attributes = new List<CustomAttributeBuilder>();
        }

        public void SetAttribute(
            Type attributeType,
            IDictionary<Type, object> ctorParametersValuesMapping,
            IDictionary<string, object> propertiesNamesValuesMapping)
        {
            var attribute = DynamicAttributeFactory.CreateAttribute(attributeType, ctorParametersValuesMapping,
                propertiesNamesValuesMapping);

            _attributes.Add(attribute);
        }

        private void DefinePropertyGetter(
            PropertyBuilder propertyBuilder,
            FieldBuilder fieldBuilder,
            TypeBuilder typeBuilder)
        {
            var getMethodBuilder = typeBuilder.DefineMethod(
                $"Get_{_propertyName}",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                CallingConventions.Standard,
                _propertyType,
                new Type[0]);

            var ilGen = getMethodBuilder.GetILGenerator();

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, fieldBuilder);
            ilGen.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);
        }

        private void DefinePropertySetter(
            PropertyBuilder propertyBuilder,
            FieldBuilder fieldBuilder,
            TypeBuilder typeBuilder)
        {
            var setMethodBuilder = typeBuilder.DefineMethod(
                $"Set_{_propertyName}",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                CallingConventions.Standard,
                typeof(void),
                new Type[] { _propertyType });

            var ilGen = setMethodBuilder.GetILGenerator();

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Stfld, fieldBuilder);
            ilGen.Emit(OpCodes.Ret);

            propertyBuilder.SetSetMethod(setMethodBuilder);
        }

        private void AddAttributes(PropertyBuilder propertyBuilder)
        {
            foreach (var attribute in _attributes)
            {
                propertyBuilder.SetCustomAttribute(attribute);
            }
        }

        void IBuildStep.Build(TypeBuilder typeBuilder)
        {
            var propertyBuilder = typeBuilder.DefineProperty(
                _propertyName,
                PropertyAttributes.None,
                _propertyType,
                new Type[0]);

            var fieldBuilder = typeBuilder.DefineField($"_{_propertyName}", _propertyType, FieldAttributes.Private);

            DefinePropertySetter(propertyBuilder, fieldBuilder, typeBuilder);

            DefinePropertyGetter(propertyBuilder, fieldBuilder, typeBuilder);

            AddAttributes(propertyBuilder);
        }
    }
}
