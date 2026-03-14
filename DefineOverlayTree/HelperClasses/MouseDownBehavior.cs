using System.Windows;
using System.Windows.Input;

namespace DefineOverlayTree.HelperClasses
{
    public static class MouseDownBehavior
    {
        private static readonly DependencyProperty MouseDownCommandProperty = DependencyProperty.RegisterAttached
            ("MouseDownCommand", typeof(ICommand), typeof(MouseDownBehavior),
            new PropertyMetadata(MouseDownCommandPropertyChangedCallBack));

        public static void SetMouseDownCommand(this UIElement inUiElement, ICommand inCommand)
        {
            inUiElement.SetValue(MouseDownCommandProperty, inCommand);
        }

        private static ICommand GetMouseDownCommand(DependencyObject inUiElement)
        {
            return (ICommand)inUiElement.GetValue(MouseDownCommandProperty);
        }

        private static void MouseDownCommandPropertyChangedCallBack(DependencyObject inDependencyObject,
            DependencyPropertyChangedEventArgs inEventArgs)
        {
            if (!(inDependencyObject is UIElement uiElement)) return;

            uiElement.MouseDown += (sender, args) =>
            {
                GetMouseDownCommand(uiElement).Execute(args);
                args.Handled = true;
            };
        }
    }
}
