namespace DefineOverlayTree.HelperClasses
{
    public interface INotifyItemPropertyChangedEx<out T>
    {
        event EventHandlerEx<IPropertyChangedEventExArgs<T>> ItemPropertyChanged;
    }

    public delegate void EventHandlerEx<in T>(object sender, T e);
}
