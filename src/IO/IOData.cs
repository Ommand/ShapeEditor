using ShapeEditor.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor.src.IO
{
    public abstract class IOData
    {
        protected GraphicsController graphicsController;

        protected Func<int, int, Point> TransformPixelToOrtho;
        protected Func<double, double, Point> TransformOrthoToPixel;

        protected Point TransformPointToPixel(Point point)
        {
            return this.TransformOrthoToPixel(point.X, point.Y);
        }
        protected Point TransformPointToOrtho(Point point)
        {
            return this.TransformPixelToOrtho((int)point.X, (int)point.Y);
        }
        protected double Norm(Point a, Point b)
        {
            double result = (Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
            return Math.Sqrt(result);
        }

        protected double ScalebleNorm(Point a, Point b, double scaleX, double scaleY)
        {
            double result = (Math.Pow((b.X - a.X) / scaleX, 2) + Math.Pow((b.Y - a.Y) / scaleY, 2));
            return Math.Sqrt(result);
        }
        protected string ColorToString(Color color)
        {
            string result = String.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
            return result;
        }
        public IOData(GraphicsController _graphicsController, Func<int, int, Point> _TransformPixelToOrtho, Func<double, double, Point> _TransformOrthoToPixel)
        {
            if (_TransformPixelToOrtho == null || _TransformOrthoToPixel == null || _graphicsController == null)
                throw new ArgumentNullException();

            this.graphicsController = _graphicsController;
            this.TransformPixelToOrtho = _TransformPixelToOrtho;
            this.TransformOrthoToPixel = _TransformOrthoToPixel;
        }
    }
}
