using System;
using System.Collections.Generic;

namespace DynamicTypeGenerator.Abstracts
{
    public interface IDynamicMethodBuilder : IDynamicAttributeSetter
    {
        IDynamicMethodBuilder SetReturnType(Type returnType);
        IDynamicMethodBuilder SetParameter(Type parameterType, string value = null);
    }
}