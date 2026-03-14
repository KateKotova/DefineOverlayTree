using System.ComponentModel;

namespace DefineOverlayTree.HelperClasses
{
    public class PropertyChangedEventExArgs<T> : PropertyChangedEventArgs, IPropertyChangedEventExArgs<T>
    {
        public virtual T Item { get; }

        public PropertyChangedEventExArgs(T item, string propertyName)
            : base(propertyName)
        {
            Item = item;
        }
    }
}
