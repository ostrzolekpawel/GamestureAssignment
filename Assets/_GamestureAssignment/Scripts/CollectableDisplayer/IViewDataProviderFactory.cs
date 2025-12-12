namespace GamestureAssignment.CollectableDisplayer
{
    public interface IViewDataProviderFactory<in TType, out IViewDataProvider>
    {
        IViewDataProvider GetViewProvider(TType type);
    }
}