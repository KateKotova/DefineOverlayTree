using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DefineOverlayTree.HelperClasses
{
    /// <summary>
    /// This class can be used in ViewModels to easily bind combobox to enum:
    ///     <ComboBox ItemsSource="{Binding prop.Items}" SelectedValue="{Binding prop}"/>
    /// It can also support custom display names for enum values - see constructor
    /// </summary>
    /// <typeparam name="T">Enum to wrap</typeparam>
    public class EnumWrapper<T>
    {
        private string m_Name;
        private ObservableCollection<EnumWrapper<T>> m_Items;

        public T Value { get; }
        public bool HasValue { get; }

        public ObservableCollection<EnumWrapper<T>> Items
        {
            get
            {
                if (m_Items != null) return m_Items;
                m_Items = new ObservableCollection<EnumWrapper<T>>();
                foreach (T eValue in Enum.GetValues(typeof(T)))
                {
                    m_Items.Add(Equals(eValue, Value) && HasValue ? this
                        : new EnumWrapper<T>(eValue, m_Items) { m_Name = eValue.ToString() });
                }
                return m_Items;
            }
        }

        private EnumWrapper(T value, ObservableCollection<EnumWrapper<T>> items)
        {
            Value = value;
            HasValue = true;
            m_Items = items;
        }

        public EnumWrapper(IEnumerable<string> names = null)
        {
            if (names == null) return;
            var i = 0;
            foreach (var name in names)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    Items[i].m_Name = name;
                }
                ++i;
            }
        }

        public EnumWrapper(T value, IEnumerable<string> names = null) : this(names)
        {
            Value = value;
            HasValue = true;
            m_Name = Value.ToString();
        }

        public EnumWrapper<T> Get(T value)
        {
            return Items.First(i => Equals(i.Value, value));
        }

        public static implicit operator T(EnumWrapper<T> wrapper)
        {
            return wrapper.Value;
        }

        public override string ToString()
        {
            return m_Name;
        }
    }
}
