using System;
using System.Collections.Generic;

namespace DynamicTypeGenerator.Tests.SampleTypes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class SampleAttribute : Attribute
    {
        private readonly string _name;
        private readonly int _number;

        public SampleAttribute(string name, int number)
        {
            _name = name;
            _number = number;
        }

        public string Message2 { get; set; }
        public string Message1 { get; set; }

        public static IDictionary<Type, object> GetCtorParamValueMapping()
            => new Dictionary<Type, object>
            {
                {typeof(string), "name" },
                {typeof(int), int.MaxValue },
            };

        public static IDictionary<string, object> GetPropertyValueMaaping()
            => new Dictionary<string, object>
            {
                {nameof(Message1), "Message1" },
                {nameof(Message2), "Message2" },
            };
    }
}
