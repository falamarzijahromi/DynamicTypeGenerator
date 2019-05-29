using System;

namespace DynamicTypeGenerator.Tests.SampleTypes
{
    public interface ISampleInterface
    {
        object DoThis(object @object, ConsoleColor color, Exception exception, string message, int index);
    }
}