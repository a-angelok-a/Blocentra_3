using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Blocentra_3.Behaviors
{
    /// <summary>
    /// A reusable behavior that allows any FrameworkElement 
    /// to act as a draggable area for its parent <see cref="Window"/>.
    /// Attach this behavior in XAML and set <see cref="EnableDrag"/> to true 
    /// to enable dragging the window by clicking and holding the element.
    /// </summary>
    public class WindowDragBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// DependencyProperty to enable or disable window dragging.
        /// </summary>
        public static readonly DependencyProperty EnableDragProperty =
            DependencyProperty.Register(
                nameof(EnableDrag),
                typeof(bool),
                typeof(WindowDragBehavior),
                new PropertyMetadata(false, OnEnableDragChanged));

        /// <summary>
        /// Gets or sets whether dragging the associated element should move the window.
        /// </summary>
        public bool EnableDrag
        {
            get => (bool)GetValue(EnableDragProperty);
            set => SetValue(EnableDragProperty, value);
        }

        /// <summary>
        /// Handles changes to the <see cref="EnableDrag"/> property.
        /// Attaches or detaches the MouseLeftButtonDown event handler accordingly.
        /// </summary>
        private static void OnEnableDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (WindowDragBehavior)d;
            if (behavior.AssociatedObject == null) return;

            if ((bool)e.NewValue)
                behavior.AssociatedObject.MouseLeftButtonDown += behavior.OnMouseLeftButtonDown;
            else
                behavior.AssociatedObject.MouseLeftButtonDown -= behavior.OnMouseLeftButtonDown;
        }

        /// <summary>
        /// Called when the behavior is attached to a FrameworkElement.
        /// Subscribes to the MouseLeftButtonDown event if <see cref="EnableDrag"/> is true.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (EnableDrag)
                AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        /// <summary>
        /// Called when the behavior is detached from its associated element.
        /// Unsubscribes from the MouseLeftButtonDown event to avoid memory leaks.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        }

        /// <summary>
        /// Initiates the drag operation when the left mouse button is pressed on the associated element.
        /// </summary>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Retrieve the Window that contains the associated element.
            var window = Window.GetWindow(AssociatedObject);

            // Perform drag only if a window was found.
            window?.DragMove();
        }
    }
}
