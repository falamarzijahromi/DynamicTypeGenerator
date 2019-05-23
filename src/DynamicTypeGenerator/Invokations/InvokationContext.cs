using System;
using System.Collections.Generic;

namespace DynamicTypeGenerator.Invokations
{
    public class InvokationContext
    {
        public InvokationContext(Type returnType, string methodName)
        {
            Parameters = new List<ArgInfo>();
            InjectedFields = new List<ArgInfo>();

            ReturnType = returnType;
            MethodName = methodName;
        }

        public void AddParameter(string paramName, Type paramType, object paramObject)
        {
            var argInfo = new ArgInfo(paramName, paramType, paramObject);

            Parameters.Add(argInfo);
        }

        public void AddField(string fieldName, Type fieldType, object fieldObject)
        {
            var argInfo = new ArgInfo(fieldName, fieldType, fieldObject);

            InjectedFields.Add(argInfo);
        }

        public List<ArgInfo> Parameters { get; }
        public List<ArgInfo> InjectedFields { get; }
        public Type ReturnType { get; }
        public string MethodName { get; }
    }
}
