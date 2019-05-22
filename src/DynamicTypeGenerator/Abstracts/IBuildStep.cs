namespace DynamicTypeGenerator.Abstracts
{
    internal interface IBuildStep
    {
        void Build(System.Reflection.Emit.TypeBuilder typeBuilder);
    }
}