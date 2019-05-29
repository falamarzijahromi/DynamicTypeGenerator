using System;
using System.Collections.Generic;
using DynamicTypeGenerator.Tests.SampleTypes;
using Xunit;

namespace DynamicTypeGenerator.Tests
{
    public class InterfaceImplementationTests
    {
        [Fact]
        public void Dynamic_Generated_Class_Must_Implemented_Specified_Interface()
        {
            var @interface = typeof(ISampleInterface);

            var classBuilder =
                DynamicTypeBuilderFactory.CreateClassBuilder(@interface.Name, @interface,
                    new Dictionary<string, Type>());

            var type = classBuilder.Build();

            AssertOnTypeHasImplementededInterface(
                type: type,
                implementedInterface: @interface);

        }

        private void AssertOnTypeHasImplementededInterface(Type type, Type implementedInterface)
        {
            Assert.True(ReflectionHelper.HasImplementedInterface(type, implementedInterface));
        }
    }
}
