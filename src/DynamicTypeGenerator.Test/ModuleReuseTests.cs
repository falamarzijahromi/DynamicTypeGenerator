using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DynamicTypeGenerator.Abstracts;
using Xunit;

namespace DynamicTypeGenerator.Tests
{
    public class ModuleReuseTests
    {
        [Fact]
        public void Only_One_Assembly_Must_Be_Loaded()
        {
            var asmName = new AssemblyName("SomeName");

            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);

            var moduleBuilder = asmBuilder.DefineDynamicModule("SomeModule");

            var builder1 = DynamicTypeBuilderFactory.CreateClassBuilder("Dynamic.TestClass1", new Dictionary<string, Type>(), moduleBuilder);
            var builder2 = DynamicTypeBuilderFactory.CreateClassBuilder("Dynamic.TestClass2", new Dictionary<string, Type>(), moduleBuilder);

            var classType1 = builder1.Build();
            var classType2 = builder2.Build();

            Assert.Equal(classType2.Assembly, classType1.Assembly);
        }
    }
}