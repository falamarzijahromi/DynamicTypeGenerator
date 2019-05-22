using System;

namespace DynamicTypeGenerator.Abstracts
{
    public interface IInvokationEvaluator
    {
    }

    public class InvokactionContext
    {
        public InvokactionContext()
        {

        }
    }

    public class InjectedField
    {
        public InjectedField(string fieldName, Type fieldType, object field)
        {
            FieldName = fieldName;
            FieldType = fieldType;
            Field = field;
        }

        public string FieldName { get; }
        public Type FieldType { get; }
        public object Field { get; }
    }

    public class MethodParameter
    {
        public MethodParameter(Type paramType, object paramObject)
        {
            ParamType = paramType;
            ParamObject = paramObject;
        }

        public Type ParamType { get; }
        public object ParamObject { get; }
    }

    public class InvokationContextFactory
    { }
}