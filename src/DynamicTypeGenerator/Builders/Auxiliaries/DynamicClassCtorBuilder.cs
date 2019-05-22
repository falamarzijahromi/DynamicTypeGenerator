using DynamicTypeGenerator.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicTypeGenerator.Builders.Auxiliaries
{
    internal class DynamicClassCtorBuilder
    {
        private readonly IDictionary<string, Type> ctorParams;

        public DynamicClassCtorBuilder(IDictionary<string, Type> ctorParams)
        {
            this.ctorParams = ctorParams;
        }

        public List<FieldBuilder> Build(TypeBuilder typeBuilder)
        {
            var @params = new List<Type> { typeof(IInvokationEvaluator) };

            @params.AddRange(ctorParams.Values.ToArray());

            var fieldBuilders = DefineFields(typeBuilder);

            DefineCtor(typeBuilder, fieldBuilders);

	        return fieldBuilders;
        }

        private List<FieldBuilder> DefineFields(TypeBuilder typeBuilder)
        {
            var evaluatorFieldBuilder = DefineEvaluatorField(typeBuilder);

            var retList = new List<FieldBuilder> { evaluatorFieldBuilder };

            foreach (var param in ctorParams.Keys)
            {
                var fieldBuilder = typeBuilder.DefineField($"_{param}", ctorParams[param],
                    FieldAttributes.Private | FieldAttributes.InitOnly);

                retList.Add(fieldBuilder);
            }

            return retList;
        }

        private FieldBuilder DefineEvaluatorField(TypeBuilder typeBuilder)
        {
            return typeBuilder.DefineField("_evaluator", typeof(IInvokationEvaluator),
                FieldAttributes.Private | FieldAttributes.InitOnly);
        }

        private void DefineCtor(TypeBuilder typeBuilder, IList<FieldBuilder> fieldBuilders)
        {
            var ctorParamsType = fieldBuilders.Select(fb => fb.FieldType).ToArray();

            var ctorBuilder =
                typeBuilder.DefineConstructor(
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    CallingConventions.Standard | CallingConventions.HasThis,
                    ctorParamsType);

            var ilGen = ctorBuilder.GetILGenerator();

            DefineCtorBody(fieldBuilders, ilGen);
        }

        private void DefineCtorBody(IList<FieldBuilder> fieldBuilders, ILGenerator ilGen)
        {
            GenerateCallObjectBaseCtor(ilGen);

            for (int i = 0; i < fieldBuilders.Count; i++)
            {
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldarg_S, i + 1);
                ilGen.Emit(OpCodes.Stfld, fieldBuilders[i]);
            }

            ilGen.Emit(OpCodes.Ret);
        }

        private void GenerateCallObjectBaseCtor(ILGenerator ctorIlGen)
        {
            var objectCtor = typeof(object).GetConstructor(new Type[0]);

            ctorIlGen.Emit(OpCodes.Ldarg_0);
            ctorIlGen.Emit(OpCodes.Call, objectCtor);
        }

    }
}