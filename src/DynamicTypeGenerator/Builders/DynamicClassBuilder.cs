﻿using DynamicTypeGenerator.Abstracts;
using DynamicTypeGenerator.Builders.Auxiliaries;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicTypeGenerator.Builders
{
    internal class DynamicClassBuilder : DynamicTypeBuilder
    {
        private readonly IList<FieldBuilder> fields;

        public DynamicClassBuilder(string className, IDictionary<string, Type> ctorParams, ModuleBuilder moduleBuilder = null , params Type[] interfaces)
        {
            moduleBuilder = moduleBuilder ?? CreateModuleBuilder();

            TypeBuilder = moduleBuilder.DefineType(
                className,
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit);

            var ctorBuilder = new DynamicClassCtorBuilder(ctorParams);

            fields = ctorBuilder.Build(TypeBuilder);

            ImplementInterfaces(interfaces);
        }

        protected override TypeBuilder TypeBuilder { get; }

        public override IDynamicMethodBuilder SetMethod(string methodName)
        {
            var methodBuilder = new DynamicClassMethodBuilder(methodName, fields);

            AddBuildStep(methodBuilder);

            return methodBuilder;
        }

        public override IDynamicPropertyBuilder SetProperty(string propertyName, Type propertyType)
        {
            throw new NotSupportedException("Property For Class Still Doesn't Requested");
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
                var methodBuilder = new DynamicClassMethodBuilder(method.Name, fields);

                methodBuilder.SetReturnType(method.ReturnType);

                AddMethodParameters(method, methodBuilder);

                AddBuildStep(methodBuilder);
            }
        }

        private void ImplementInterfaces(Type[] interfaces)
        {
            foreach (var @interface in interfaces)
            {
                AddInterfaceImplementationSteps(@interface);
            }
        }

        private void AddMethodParameters(MethodInfo method, DynamicClassMethodBuilder methodBuilder)
        {
            foreach (var param in method.GetParameters())
            {
                methodBuilder.SetParameter(param.ParameterType, param.Name);
            }
        }
    }
}
