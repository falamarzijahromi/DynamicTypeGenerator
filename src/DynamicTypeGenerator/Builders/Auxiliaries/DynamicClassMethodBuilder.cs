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

			ilGen.Emit(OpCodes.Ldarg_0);

			var invokationContextVariable = DefineInvokationContext(ilGen);

			//AddAllFields(ilGen, invokationContextVariable);

			AddAllParams(ilGen, invokationContextVariable);

			//CallInvokationEvaluator(ilGen, invokationContextVariable);

			ilGen.Emit(OpCodes.Ret);
		}

		private static LocalBuilder DefineInvokationContext(ILGenerator ilGen)
		{
			var invokationContextVariable = ilGen.DeclareLocal(typeof(InvokactionContext));

			return invokationContextVariable;
		}

		private void CallInvokationEvaluator(ILGenerator ilGen, LocalBuilder localVariable)
		{
			var evaluateMethod = typeof(IInvokationEvaluator).GetMethod(nameof(IInvokationEvaluator.Evaluate));

			ilGen.Emit(OpCodes.Ldarg_0);
			ilGen.Emit(OpCodes.Ldfld, fields[0]);

			PushInvokationContextToStack(ilGen, localVariable);

			ilGen.Emit(OpCodes.Call, evaluateMethod);
		}

		private void PushInvokationContextToStack(ILGenerator ilGen, LocalBuilder localVariable)
		{
			ilGen.Emit(OpCodes.Ldloc, localVariable);
		}

		private void AddAllParams(ILGenerator ilGen, LocalBuilder localVariable)
		{
			var addParamMethod = localVariable.LocalType.GetMethod(nameof(InvokactionContext.AddParameter), BindingFlags.NonPublic | BindingFlags.Instance);

			for (int i = 0; i < @params.Count; i++)
			{
				ilGen.Emit(OpCodes.Ldarg_S, i + 1);

				PushInvokationContextToStack(ilGen, localVariable);

				ilGen.Emit(OpCodes.Call, addParamMethod);
			}
		}

		private void AddAllFields(ILGenerator ilGen, LocalBuilder localVariable)
		{
			var addFieldMethod = localVariable.LocalType.GetMethod(nameof(InvokactionContext.AddField), BindingFlags.NonPublic | BindingFlags.Instance);

			for (int i = 1; i < fields.Count; i++)
			{
				ilGen.Emit(OpCodes.Ldarg_0);
				ilGen.Emit(OpCodes.Ldfld, fields[i]);

				PushInvokationContextToStack(ilGen, localVariable);

				ilGen.Emit(OpCodes.Call, addFieldMethod);
			}
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