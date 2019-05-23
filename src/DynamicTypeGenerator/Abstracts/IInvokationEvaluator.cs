using DynamicTypeGenerator.Invokations;

namespace DynamicTypeGenerator.Abstracts
{
    public interface IInvokationEvaluator
    {
        object Evaluate(InvokationContext context);
    }
}