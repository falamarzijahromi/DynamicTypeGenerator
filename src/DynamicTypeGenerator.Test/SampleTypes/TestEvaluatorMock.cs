using System;
using DynamicTypeGenerator.Abstracts;
using DynamicTypeGenerator.Invokations;

namespace DynamicTypeGenerator.Tests.SampleTypes
{
    public class TestEvaluatorMock : IInvokationEvaluator
    {
        private InvokationContext context;

        public object Evaluate(InvokationContext context)
        {
            this.context = context;

            object retValue = null;

            if (context.ReturnType.IsValueType)
            {
                retValue = Activator.CreateInstance(context.ReturnType);
            }

            return retValue;
        }

        public void AssertInvokationContext()
        {
            if (context == null)
            {
                throw new Exception("No context received");
            }


        }
    }
}
