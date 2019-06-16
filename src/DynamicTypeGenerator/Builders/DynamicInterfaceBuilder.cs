using System;
using System.Reflection;
using System.Reflection.Emit;
using DynamicTypeGenerator.Abstracts;
using DynamicTypeGenerator.Builders.Auxiliaries;

namespace DynamicTypeGenerator.Builders
{
    internal class DynamicInterfaceBuilder : DynamicTypeBuilder
    {
        private readonly string interfaceFullName;

        internal DynamicInterfaceBuilder(string interfaceFullName, ModuleBuilder moduleBuilder = null, params Type[] interfaces)
        {
            this.interfaceFullName = interfaceFullName;

            moduleBuilder = moduleBuilder ?? CreateModuleBuilder();

            TypeBuilder = moduleBuilder.DefineType(interfaceFullName, TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

            ImplementInterfaces(interfaces);
        }

        protected override TypeBuilder TypeBuilder { get; }

        public override IDynamicMethodBuilder SetMethod(string methodName)
        {
            var methodBuilder = new DynamicInterfaceMethodBuilder(methodName);

            AddBuildStep(methodBuilder);

            return methodBuilder;
        }

        public override IDynamicPropertyBuilder SetProperty(string propertyName, Type propertyType)
        {
            throw new NotSupportedException("Property For Interface Still Doesn't Requested");
        }

        private void ImplementInterfaces(Type[] interfaces)
        {
            foreach (var @interface in interfaces)
            {
                AddInterfaceImplementationSteps(@interface);
            }
        }

        private void AddInterfaceImplementationSteps(Type @interface)
        {
            if (@interface != null)
            {
                TypeBuilder.AddInterfaceImplementation(@interface);

                AddInterfaceMethodBuildSteps(@interface);
            }
        }

        private void AddInterfaceMethodBuildSteps(Type @interface)
        {
            var interfaceMethods = @interface.GetMethods();

            foreach (var method in interfaceMethods)
            {
                var methodBuilder = new DynamicInterfaceMethodBuilder(method.Name);

                methodBuilder.SetReturnType(method.ReturnType);

                AddMethodParameters(method, methodBuilder);

                AddBuildStep(methodBuilder);
            }
        }

        private void AddMethodParameters(MethodInfo method, DynamicInterfaceMethodBuilder methodBuilder)
        {
            foreach (var param in method.GetParameters())
            {
                methodBuilder.SetParameter(param.ParameterType, param.Name);
            }
        }
    }
}
