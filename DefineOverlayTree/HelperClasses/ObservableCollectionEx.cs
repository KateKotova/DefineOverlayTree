using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace DefineOverlayTree.HelperClasses
{
    [Serializable]
    public class ObservableCollectionEx<T> : ObservableCollection<T>, INotifyItemPropertyChangedEx<T>
        where T : INotifyPropertyChanged
    {
        public event EventHandlerEx<IPropertyChangedEventExArgs<T>> ItemPropertyChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            Unsubscribe(e.OldItems);
            Subscribe(e.NewItems);
            base.OnCollectionChanged(e);
        }

        public ObservableCollectionEx(IEnumerable<T> list) : base(list)
        {
        }

        public ObservableCollectionEx()
        {
        }

        protected override void ClearItems()
        {
            foreach (T element in this)
            {
                element.PropertyChanged -= ContainedElementChanged;
            }

            base.ClearItems();
        }

        public void Update(T[] items)
        {
            // Use ToList to allow modification of ModelParametersObservable in the loop.
            foreach (var p in this.Except(items).ToList())
            {
                Remove(p);
            }
            foreach (var p in items.Except(this).ToList())
            {
                Add(p);
            }
        }

        private void Subscribe(IList iList)
        {
            if (iList != null)
            {
                foreach (T element in iList)
                {
                    element.PropertyChanged += ContainedElementChanged;
                }
            }
        }

        private void Unsubscribe(IList iList)
        {
            if (iList != null)
            {
                foreach (T element in iList)
                {
                    element.PropertyChanged -= ContainedElementChanged;
                }
            }
        }

        public void AddRange(IList iList)
        {
            if (iList != null)
            {
                foreach (T element in iList)
                {
                    Items.Add(element);
                    element.PropertyChanged += ContainedElementChanged;
                }

                if (iList.Count > 0)
                {
                    base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                        iList));
                }
            }
        }

        public void RemoveRange(IList iList)
        {
            if (iList != null)
            {
                foreach (T element in iList)
                {
                    element.PropertyChanged -= ContainedElementChanged;
                    Items.Remove(element);
                }

                if (iList.Count > 0)
                {
                    base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, iList));
                }
            }
        }

        protected virtual void ContainedElementChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
            ItemPropertyChanged?.Invoke(this, new PropertyChangedEventExArgs<T>((T)sender, e.PropertyName));
        }
    }
}
