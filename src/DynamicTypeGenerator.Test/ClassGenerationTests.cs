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
                { "_evaluator", new TestEvaluatorMock()},
                { "_arg1", "SampleString"},
                { "_arg2", 456},
                { "_arg3", Guid.NewGuid()},
            };

            var builder = DynamicTypeBuilderFactory.CreateClassBuilder("Dynamic.TestClass", ctorTypeMapping);

            var classType = builder.Build();

            AssertInstaciatedObjectHavingSpecifiedFieldValues(classType, fieldValues);
        }

        [Theory]

        [InlineData(typeof(int))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(object))]
        [InlineData(typeof(string))]
        [InlineData(typeof(Exception))]
        [InlineData(typeof(ConsoleColor))]

        [InlineData(typeof(ConsoleColor[]))]
        [InlineData(typeof(Exception[]))]
        [InlineData(typeof(Guid[]))]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(string[]))]
        [InlineData(typeof(object[]))]

        [InlineData(typeof(IEnumerable<object>))]
        [InlineData(typeof(IEnumerable<int>))]
        [InlineData(typeof(IEnumerable<string>))]
        [InlineData(typeof(IEnumerable<Guid>))]
        [InlineData(typeof(IEnumerable<Exception>))]
        [InlineData(typeof(IEnumerable<ConsoleColor>))]

        [InlineData(typeof(ICollection<object>))]
        [InlineData(typeof(ICollection<int>))]
        [InlineData(typeof(ICollection<string>))]
        [InlineData(typeof(ICollection<Guid>))]
        [InlineData(typeof(ICollection<Exception>))]
        [InlineData(typeof(ICollection<ConsoleColor>))]

        [InlineData(typeof(List<object>))]
        [InlineData(typeof(List<int>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(List<Guid>))]
        [InlineData(typeof(List<Exception>))]
        [InlineData(typeof(List<ConsoleColor>))]

        public void Dynamic_Class_Must_Be_Created_With_The_Defined_Working_Method_With_Specified_Return_Type(Type returnType)
        {
            var method1Name = "Method1";
            var method1ReturnType = returnType;
            var method1Params = new List<Type> { typeof(int), typeof(string), typeof(Exception) };

            var testEvaluator = new TestEvaluatorMock();

            var ctorParamValueMapping = new Dictionary<Type, object>
            {
                {typeof(IInvokationEvaluator), testEvaluator},
                {typeof(string), "someString"},
            };

            var paramsValueMapping = new Dictionary<Type, object>
            {
                {typeof(int), 21},
                {typeof(string), "sdf"},
                {typeof(Exception), new Exception("Hello Evaluator")},
            };

            var builder = DynamicTypeBuilderFactory.CreateClassBuilder("Dynamic.TestClass", new Dictionary<string, Type> { { "someId", typeof(string) } });

            SetMethod(builder, method1Name, method1ReturnType, method1Params);

            var classType = builder.Build();

            CallMethodOfTypeWithParams(
                method1Name,
                classType,
                ctorParamValueMapping,
                paramsValueMapping);

            testEvaluator.AssertInvokationContext();
        }

        [Fact]
        public void Dynamic_Class_Must_Be_Created_With_Specified_Method_And_Its_Attribute()
        {
            var method1Name = "Method1";
            var method1ReturnType = typeof(string);
            var method1Params = new List<Type> { typeof(int), typeof(string) };
            var attributeType = typeof(SampleAttribute);
            var ctorParamsMapping = SampleAttribute.GetCtorParamValueMapping();
            var propsValuesMapping = SampleAttribute.GetPropertyValueMaaping();
            var setAttributeParam = new Dictionary<Type, Tuple<IDictionary<Type, object>, IDictionary<string, object>>>
            {
                {attributeType,  new Tuple<IDictionary<Type, object>, IDictionary<string, object>>(ctorParamsMapping, propsValuesMapping) },
            };


            var builder = DynamicTypeBuilderFactory.CreateClassBuilder("Dynamic.TestClass", new Dictionary<string, Type>());

            SetMethod(builder, method1Name, method1ReturnType, method1Params, setAttributeParam);

            var classType = builder.Build();

            AssertOnHavingMethodWithAttribute(
                classType: classType,
                methodName: method1Name,
                returnType: method1ReturnType,
                @params: method1Params,
                propValuesMapping: new Dictionary<Type, IDictionary<string, object>>
                {
                    {attributeType, propsValuesMapping },
                });
        }

        [Fact]
        public void Dynamic_Class_Must_Be_Created_With_Specified_Method_With_Its_Parameter_Names()
        {
            var method1Name = "Method1";
            var method1ReturnType = typeof(string);
            var method1Params = new Dictionary<Type, string>
            {
                {typeof(int), "number"},
                {typeof(string), "message"},
            };
            var attributeType = typeof(SampleAttribute);
            var ctorParamsMapping = SampleAttribute.GetCtorParamValueMapping();
            var propsValuesMapping = SampleAttribute.GetPropertyValueMaaping();
            var setAttributeParam = new Dictionary<Type, Tuple<IDictionary<Type, object>, IDictionary<string, object>>>
            {
                {attributeType,  new Tuple<IDictionary<Type, object>, IDictionary<string, object>>(ctorParamsMapping, propsValuesMapping) },
            };


            var builder = DynamicTypeBuilderFactory.CreateClassBuilder("Dynamic.TestClass", new Dictionary<string, Type>());

            SetMethod(builder, method1Name, method1ReturnType, method1Params, setAttributeParam);

            var classType = builder.Build();

            AssertOnHavingMethodWithParamNames(
                classType: classType,
                methodName: method1Name,
                returnType: method1ReturnType,
                @params: method1Params,
                propValuesMapping: new Dictionary<Type, IDictionary<string, object>>
                {
                    {attributeType, propsValuesMapping },
                });
        }

        private void AssertOnHavingMethodWithAttribute(Type classType, string methodName, Type returnType, List<Type> @params, IDictionary<Type, IDictionary<string, object>> propValuesMapping)
        {
            AssertOnHavingMethod(classType, methodName, returnType, @params);

            AssertMethodHasAttributes(classType, methodName, propValuesMapping);
        }

        private void SetMethod(IDynamicTypeBuilder builder, string method1Name, Type method1ReturnType, Dictionary<Type, string> method1Params, Dictionary<Type, Tuple<IDictionary<Type, object>, IDictionary<string, object>>> setAttributeParam)
        {
            var methodBuilder = builder.SetMethod(method1Name);

            foreach (var param in method1Params)
            {
                methodBuilder.SetParameter(param.Key, param.Value);
            }

            methodBuilder.SetReturnType(method1ReturnType);

            if (setAttributeParam != null)
            {
                foreach (var attribute in setAttributeParam)
                {
                    methodBuilder.SetAttribute(attribute.Key, attribute.Value.Item1, attribute.Value.Item2);
                }
            }
        }

        private void AssertOnHavingMethodWithParamNames(Type classType, string methodName, Type returnType, Dictionary<Type, string> @params, IDictionary<Type, IDictionary<string, object>> propValuesMapping)
        {
            Assert.True(ReflectionHelper.HasMethod(classType, methodName, returnType, @params));
        }

        private void AssertMethodHasAttributes(Type classType, string methodName, IDictionary<Type, IDictionary<string, object>> propValuesMapping)
        {
            Assert.True(ReflectionHelper.MethodHasAttributes(classType, methodName, propValuesMapping));
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
            IList<Type> @params,
            IDictionary<Type, Tuple<IDictionary<Type, object>, IDictionary<string, object>>> attributes = null)
        {
            var methodBuilder = classBuilder.SetMethod(methodName);

            foreach (var param in @params)
            {
                methodBuilder.SetParameter(param);
            }

            methodBuilder.SetReturnType(returnType);

            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    methodBuilder.SetAttribute(attribute.Key, attribute.Value.Item1, attribute.Value.Item2);
                }
            }
        }

        private void AssertOnHavingMethod(Type classType, string methodName, Type returnType, IList<Type> @params)
        {
            Assert.True(ReflectionHelper.HasMethod(classType, methodName, returnType, @params));
        }
    }
}
