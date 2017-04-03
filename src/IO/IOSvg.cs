using ShapeEditor.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeEditor.src.IO
{
    class IOSvg: IOShapeEditor
    {
        #region SaveMethods
        private int saveEllipse()
        {
            return 0;
        }

        private int saveLine()
        {
            return 0;
        }

        private int saveQuadrangle()
        {
            return 0;
        }

        private int saveTriangle()
        {
            return 0;
        }

        #endregion

        #region LoadMethods
        private int loadEllipse()
        {
            return 0;
        }

        private int loadLine()
        {
            return 0;
        }

        private int loadQuadrangle()
        {
            return 0;
        }

        private int loadTriangle()
        {
            return 0;
        }

        #endregion

        #region Methods

        int IOShapeEditor.saveShapes(List<Shape> shapes, string filePath)
        {
            foreach (Shape item in shapes)
            {
                if (item is Ellipse) saveEllipse();
                else if (item is Line) saveLine();
                else if (item is Triangle) saveTriangle();
                else if (item is Quadrangle) saveQuadrangle();
            }

            return 0;
        }

        List<Shape> IOShapeEditor.loadShapes(string filePath)
        {
            return null;
        }

        #endregion
    }
}
