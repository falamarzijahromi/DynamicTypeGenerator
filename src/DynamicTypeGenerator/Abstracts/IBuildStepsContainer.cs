namespace DynamicTypeGenerator.Abstracts
{
    internal interface IBuildStepsContainer
    {
        void AddBuildStep(IBuildStep buildStep);
    }
}