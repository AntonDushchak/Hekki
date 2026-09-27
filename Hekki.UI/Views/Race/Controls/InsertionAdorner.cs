using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Hekki.UI.Views.Race.Controls
{
    public class InsertionAdorner : Adorner
    {
        private readonly Pen _pen;
        private double _offsetY;

        public InsertionAdorner(UIElement adornedElement, Brush brush) : base(adornedElement)
        {
            IsHitTestVisible = false;
            _pen = new Pen(brush, 2);
            _pen.Freeze();
        }

        public double OffsetY
        {
            get => _offsetY;
            set
            {
                if (_offsetY == value) return;
                _offsetY = value;
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawLine(_pen, new Point(0, OffsetY), new Point(AdornedElement.RenderSize.Width, OffsetY));
        }
    }
}
