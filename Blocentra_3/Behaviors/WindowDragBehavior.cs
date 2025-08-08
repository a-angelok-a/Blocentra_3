using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Blocentra_3.Behaviors
{
    public class WindowDragBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty EnableDragProperty =
            DependencyProperty.Register(nameof(EnableDrag), typeof(bool), typeof(WindowDragBehavior),
                new PropertyMetadata(false, OnEnableDragChanged));

        public bool EnableDrag
        {
            get => (bool)GetValue(EnableDragProperty);
            set => SetValue(EnableDragProperty, value);
        }

        private static void OnEnableDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (WindowDragBehavior)d;
            if (behavior.AssociatedObject == null) return;

            if ((bool)e.NewValue)
                behavior.AssociatedObject.MouseLeftButtonDown += behavior.OnMouseLeftButtonDown;
            else
                behavior.AssociatedObject.MouseLeftButtonDown -= behavior.OnMouseLeftButtonDown;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (EnableDrag)
                AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(AssociatedObject);
            window?.DragMove();
        }
    }
}
