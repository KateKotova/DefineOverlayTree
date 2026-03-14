namespace DefineOverlayTree.HelperClasses
{
    public interface IPropertyChangedEventExArgs<out T>
    {
        T Item { get; }
        string PropertyName { get; }
    }
}
