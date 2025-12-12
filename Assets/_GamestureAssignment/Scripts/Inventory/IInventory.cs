namespace GamestureAssignment.CollectableCollector
{
    public interface IInventory<TData, TType>
    {
        TData Get(TType type);
        void Add(TData element);
        void Remove(TData element);
        void Set(TData element);
    }
}