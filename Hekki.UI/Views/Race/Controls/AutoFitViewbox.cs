using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hekki.UI.Views.Race.Controls
{
    public class AutoFitViewbox : Decorator
    {
        public static readonly DependencyProperty MinScaleProperty =
            DependencyProperty.Register(
                nameof(MinScale),
                typeof(double),
                typeof(AutoFitViewbox),
                new FrameworkPropertyMetadata(0.7, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty AvailableHeightProperty =
            DependencyProperty.Register(
                nameof(AvailableHeight),
                typeof(double),
                typeof(AutoFitViewbox),
                new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.AffectsMeasure));

        private double _scale = 1.0;

        public double MinScale
        {
            get => (double)GetValue(MinScaleProperty);
            set => SetValue(MinScaleProperty, value);
        }

        public double AvailableHeight
        {
            get => (double)GetValue(AvailableHeightProperty);
            set => SetValue(AvailableHeightProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Child is null)
                return new Size();

            Child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var contentSize = Child.DesiredSize;
            var availableHeight = AvailableHeight;

            _scale = 1.0;
            if (contentSize.Height > 0 && availableHeight > 0 && !double.IsInfinity(availableHeight))
            {
                var fitScale = availableHeight / contentSize.Height;
                _scale = Math.Max(MinScale, Math.Min(1.0, fitScale));
            }

            return new Size(contentSize.Width * _scale, contentSize.Height * _scale);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (Child is null)
                return arrangeSize;

            Child.Arrange(new Rect(0, 0, Child.DesiredSize.Width, Child.DesiredSize.Height));
            Child.RenderTransform = new ScaleTransform(_scale, _scale);
            Child.RenderTransformOrigin = new Point(0, 0);

            return arrangeSize;
        }
    }
}
