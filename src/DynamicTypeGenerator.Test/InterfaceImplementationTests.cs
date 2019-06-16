using System;
using System.Collections.Generic;
using DynamicTypeGenerator.Tests.SampleTypes;
using Xunit;

namespace DynamicTypeGenerator.Tests
{
    public class InterfaceImplementationTests
    {
        [Fact]
        public void Dynamic_Generated_Class_Must_Implemented_Specified_Interfaces()
        {
            var @interface = typeof(ISampleInterface);
            var @interface2 = typeof(ISampleInterface2);

            var classBuilder =
                DynamicTypeBuilderFactory.CreateClassBuilder(
                    @interface.Name, 
                    new Dictionary<string, Type>(), null,
                    @interface, @interface2);

            var type = classBuilder.Build();

            AssertOnTypeHasImplementededInterface(
                type: type,
                implementedInterface: @interface);

            AssertOnTypeHasImplementededInterface(
                type: type,
                implementedInterface: @interface2);
        }

        [Fact]
        public void Dynamic_Generated_Interface_Must_Implemented_Specified_Interfaces()
        {
            var @interface = typeof(ISampleInterface);
            var @interface2 = typeof(ISampleInterface2);

            var classBuilder =
                DynamicTypeBuilderFactory.CreateInterfaceBuilder(
                    "DynamicInterface", null,
                    @interface, @interface2);

            var type = classBuilder.Build();

            AssertOnTypeHasImplementededInterface(
                type: type,
                implementedInterface: @interface);

            AssertOnTypeHasImplementededInterface(
                type: type,
                implementedInterface: @interface2);
        }

        private void AssertOnTypeHasImplementededInterface(Type type, Type implementedInterface)
        {
            Assert.True(ReflectionHelper.HasImplementedInterface(type, implementedInterface));
        }
    }
}
