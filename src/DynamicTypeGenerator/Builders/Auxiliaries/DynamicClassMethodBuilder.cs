using DynamicTypeGenerator.Abstracts;
using DynamicTypeGenerator.Invokations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicTypeGenerator.Builders.Auxiliaries
{
    internal class DynamicClassMethodBuilder : IDynamicMethodBuilder, IBuildStep
    {
        private readonly string methodName;
        private readonly IList<CustomAttributeBuilder> attributes;
        private readonly IList<Type> @params;
        private readonly IList<FieldBuilder> fields;

        private Type returnType;


        public DynamicClassMethodBuilder(string methodName, IList<FieldBuilder> fields)
        {
            this.methodName = methodName;
            attributes = new List<CustomAttributeBuilder>();
            @params = new List<Type>();

            returnType = typeof(void);

            this.fields = fields;
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

            ilGen.Emit(OpCodes.Nop);

            var invokationContextVariable = InstanciateInvokationContext(ilGen);

            AddFieldsToContext(ilGen, invokationContextVariable);

            AddParamsToContext(ilGen, invokationContextVariable);

            var invokeResultVariable = InvokeEvaluator(ilGen, invokationContextVariable);

            EvaluateMethodReturn(ilGen, invokeResultVariable);

            ilGen.Emit(OpCodes.Ret);
        }

        private void EvaluateMethodReturn(ILGenerator ilGen, LocalBuilder invokeResultVariable)
        {
            if (returnType.Equals(typeof(void)))
            {
                //ilGen.Emit(OpCodes.Pop);
            }
            else
            {
                var castedInvokationResultVariable = ilGen.DeclareLocal(returnType);

                ilGen.Emit(OpCodes.Ldloc, invokeResultVariable);
                ilGen.Emit(OpCodes.Castclass, returnType);

                ilGen.Emit(OpCodes.Stloc, castedInvokationResultVariable);

                ilGen.Emit(OpCodes.Br_S);

                ilGen.Emit(OpCodes.Ldloc, castedInvokationResultVariable);
            }
        }

        private LocalBuilder InvokeEvaluator(ILGenerator ilGen, LocalBuilder invokationContextVariable)
        {
            var invokeResultVariable = ilGen.DeclareLocal(typeof(object));
            var invokeMethod = typeof(IInvokationEvaluator).GetMethod(nameof(IInvokationEvaluator.Evaluate));

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, fields[0]);
            ilGen.Emit(OpCodes.Ldloc, invokationContextVariable);

            ilGen.Emit(OpCodes.Callvirt, invokeMethod);
            ilGen.Emit(OpCodes.Stloc, invokeResultVariable);

            return invokeResultVariable;
        }

        private void AddParamsToContext(ILGenerator ilGen, LocalBuilder invokationContextVariable)
        {
            var addParamMethod = typeof(InvokationContext).GetMethod(nameof(InvokationContext.AddParameter),
                BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < @params.Count; i++)
            {
                var param = @params[i];

                ilGen.Emit(OpCodes.Ldloc, invokationContextVariable);

                ilGen.Emit(OpCodes.Ldstr, $"arg_{i}");

                PushTypeToStack(ilGen, param);

                ilGen.Emit(OpCodes.Ldarg, i + 1);

                BoxIfRequired(ilGen, param);

                ilGen.Emit(OpCodes.Callvirt, addParamMethod);

                ilGen.Emit(OpCodes.Nop);
            }
        }

        private void AddFieldsToContext(ILGenerator ilGen, LocalBuilder invokationContextVariable)
        {
            var addFieldMethod = typeof(InvokationContext).GetMethod(nameof(InvokationContext.AddField),
                BindingFlags.Instance | BindingFlags.Public);

            for (int i = 1; i < fields.Count; i++)
            {
                var field = fields[i];

                ilGen.Emit(OpCodes.Ldloc, invokationContextVariable);

                ilGen.Emit(OpCodes.Ldstr, field.Name);

                PushTypeToStack(ilGen, field.FieldType);

                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldfld, field);

                BoxIfRequired(ilGen, field.FieldType);

                ilGen.Emit(OpCodes.Callvirt, addFieldMethod);

                ilGen.Emit(OpCodes.Nop);
            }
        }

        private void BoxIfRequired(ILGenerator ilGen, Type fieldType)
        {
            if (fieldType.IsValueType)
            {
                ilGen.Emit(OpCodes.Box, fieldType);
            }
        }

        private LocalBuilder InstanciateInvokationContext(ILGenerator ilGen)
        {
            var invokationContextVariable = ilGen.DeclareLocal(typeof(InvokationContext));
            var invokationContextCtor = typeof(InvokationContext).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new Type[] { typeof(Type), typeof(string) },
                new ParameterModifier[0]);

            PushTypeToStack(ilGen, returnType);

            ilGen.Emit(OpCodes.Ldstr, methodName);

            ilGen.Emit(OpCodes.Newobj, invokationContextCtor);

            ilGen.Emit(OpCodes.Stloc, invokationContextVariable);

            return invokationContextVariable;
        }

        private void PushTypeToStack(ILGenerator ilGen, Type type)
        {
            var getTypeFromHandleMethod = typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle));

            ilGen.Emit(OpCodes.Ldtoken, type);
            ilGen.Emit(OpCodes.Call, getTypeFromHandleMethod);
        }

        private MethodBuilder DefineMethod(TypeBuilder typeBuilder)
        {
            return typeBuilder.DefineMethod(
                methodName,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                CallingConventions.HasThis, returnType,
                @params.ToArray());
        }
    }
}