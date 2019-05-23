using DynamicTypeGenerator.Abstracts;
using DynamicTypeGenerator.Tests.SampleTypes;
using System;
using System.Collections.Generic;
using Xunit;

namespace DynamicTypeGenerator.Tests
{
    public class ClassGenerationTests
    {
        [Fact]
        public void Dynamic_Class_Must_Be_Created_With_Defined_Methods()
        {
            var method1Name = "Method1";
            var method1ReturnType = typeof(string);
            var method1Params = new List<Type> { typeof(int), typeof(string) };

            var method2Name = "Method2";
            var method2ReturnType = typeof(void);
            var method2Params = new List<Type> { typeof(object) };

            var builder = DynamicTypeBuilderFactory.CreateClassBuilder("Dynamic.TestClass", new Dictionary<string, Type>());

            SetMethod(builder, method1Name, method1ReturnType, method1Params);
            SetMethod(builder, method2Name, method2ReturnType, method2Params);

            var classType = builder.Build();

            AssertOnHavingMethod(
                classType: classType,
                methodName: method1Name,
                returnType: method1ReturnType,
                @params: method1Params);

            AssertOnHavingMethod(
                classType: classType,
                methodName: method2Name,
                returnType: method2ReturnType,
                @params: method2Params);
        }

        [Fact]
        public void Dynamic_Class_Must_Have_The_Ctor_With_Specified_Params_Plus_Invokaction_Evaluator()
        {
            var ctorTypeMapping = new Dictionary<string, Type>
            {
                { "arg1", typeof(string)},
                { "arg2", typeof(int)},
                { "arg3", typeof(Guid)},
            };

            var builder = DynamicTypeBuilderFactory.CreateClassBuilder("Dynamic.TestClass", ctorTypeMapping);

            var classType = builder.Build();

            var allCtorParams = new List<Type> { typeof(IInvokationEvaluator) };

            allCtorParams.AddRange(ctorTypeMapping.Values);

            AssertOnHavingCtorWithSpecifiedArgs(classType, allCtorParams.ToArray());
        }

        [Fact]
        public void Dynamic_Class_Must_Instanciated_With_Specified_Field_Values()
        {
            var ctorTypeMapping = new Dictionary<string, Type>
            {
                { "arg1", typeof(string)},
                { "arg2", typeof(int)},
                { "arg3", typeof(Guid)},
            };

            var fieldValues = new Dictionary<string, object>
            {
                { "_evaluator", new TestEvaluator()},
                { "_arg1", "SampleString"},
                { "_arg2", 456},
                { "_arg3", Guid.NewGuid()},
            };

            var builder = DynamicTypeBuilderFactory.CreateClassBuilder("Dynamic.TestClass", ctorTypeMapping);

            var classType = builder.Build();

            AssertInstaciatedObjectHavingSpecifiedFieldValues(classType, fieldValues);
        }

	    [Fact]
	    public void Dynamic_Class_Must_Be_Created_With_The_Defined_Working_Method()
	    {
		    var method1Name = "Method1";
		    var method1ReturnType = typeof(void);
		    var method1Params = new List<Type> { typeof(int), typeof(string) };

			var testEvaluator = new TestEvaluator();

		    var ctorParamValueMapping = new Dictionary<Type, object>
		    {
			    {typeof(IInvokationEvaluator), testEvaluator},
			    {typeof(string), "Hello"},
            };

		    var paramsValueMapping = new Dictionary<Type, object>
		    {
			    {typeof(int), 21},
			    {typeof(string), "sdf"},
			};

		    var builder = DynamicTypeBuilderFactory.CreateClassBuilder("Dynamic.TestClass", new Dictionary<string, Type>{{"manamHaji", typeof(string)}});

		    SetMethod(builder, method1Name, method1ReturnType, method1Params);

		    var classType = builder.Build();

		    CallMethodOfTypeWithParams(
				method1Name,
			    classType,
			    ctorParamValueMapping,
			    paramsValueMapping);
	    }

	    private void CallMethodOfTypeWithParams(
			string methodName,
		    Type classType,
		    Dictionary<Type, object> ctorParamValueMapping, 
		    Dictionary<Type, object> paramsValueMapping)
	    {
		    ReflectionHelper.ExecuteMethod(classType, methodName, ctorParamValueMapping, paramsValueMapping);
	    }

	    private void AssertInstaciatedObjectHavingSpecifiedFieldValues(Type classType, Dictionary<string, object> fieldValues)
        {
            Assert.True(ReflectionHelper.InstanciatedObjectHasSpecifiedFieldValues(classType, fieldValues));
        }

        private void AssertOnHavingCtorWithSpecifiedArgs(Type classType, params Type[] ctorTypes)
        {
            Assert.True(ReflectionHelper.HasCtorWithSpecifiedParamsType(classType, ctorTypes));
        }

        private void SetMethod(
            IDynamicTypeBuilder classBuilder,
            string methodName,
            Type returnType,
            IList<Type> @params)
        {
            var methodBuilder = classBuilder.SetMethod(methodName);

            foreach (var param in @params)
            {
                methodBuilder.SetParameter(param);
            }

            methodBuilder.SetReturnType(returnType);
        }

        private void AssertOnHavingMethod(Type classType, string methodName, Type returnType, IList<Type> @params)
        {
            Assert.True(ReflectionHelper.HasMethod(classType, methodName, returnType, @params));
        }
    }
}
