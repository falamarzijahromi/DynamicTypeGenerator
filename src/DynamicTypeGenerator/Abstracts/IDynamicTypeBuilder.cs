using System;
using System.Collections.Generic;

namespace DynamicTypeGenerator.Abstracts
{
    public interface IDynamicTypeBuilder : IDynamicAttributeSetter
    {
        IDynamicPropertyBuilder SetProperty(string propertyName, Type propertyType);

        IDynamicMethodBuilder SetMethod(string methodName);

        Type Build();
    }
}
