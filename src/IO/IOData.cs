using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapeEditor.src.IO
{
    public abstract class IOData
    {
        protected Func<int, int, Point> TransformPixelToOrtho;
        protected Func<double, double, Point> TransformOrthoToPixel;
        public IOData(Func<int, int, Point> _TransformPixelToOrtho, Func<double, double, Point> _TransformOrthoToPixel)
        {
            if (_TransformPixelToOrtho == null || _TransformOrthoToPixel == null)
                throw new ArgumentNullException();
            this.TransformPixelToOrtho = _TransformPixelToOrtho;
            this.TransformOrthoToPixel = _TransformOrthoToPixel;
        }
    }
}
