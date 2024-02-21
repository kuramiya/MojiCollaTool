using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace MojiCollaTool
{
    public class BlurredElement : FrameworkElement
    {
        private Action<DrawingContext> _action;
        private double _radius;

        public BlurredElement(Action<DrawingContext> action, double radius)
        {
            this._action = action;
            _radius = radius;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            _action(drawingContext);
            Effect = new BlurEffect
            {
                Radius = _radius,
            };
        }
    }
}
