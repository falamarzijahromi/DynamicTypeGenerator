using System;

namespace DynamicTypeGenerator.Invokations
{
    public class ArgInfo
    {
        internal ArgInfo(string paramName, Type paramType, object paramObject)
        {
            ParamName = paramName;
            ParamType = paramType;
            ParamObject = paramObject;
        }

        public string ParamName { get; }
        public Type ParamType { get; }
        public object ParamObject { get; }
    }
}
