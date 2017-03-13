using System;
using System.Collections.Generic;
using System.Linq;
using ShapeEditor.Shapes;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor.Fabrics
{
    public static class ShapeFabric
    {
        public static Shape CreateShape(ShapeTypes.ShapeType shape, IEnumerable<Point> points)
        {
            switch (shape)
            {
                case ShapeTypes.ShapeType.Triangle_:
                    var ptsTriangle = points as Point[] ?? points.ToArray();
                    if (ptsTriangle.Length < 3)
                        throw new Exception("Cannot create triangle: points array has less than 3 points");
                    return new Triangle(ptsTriangle[0], ptsTriangle[1], ptsTriangle[2]);

                case ShapeTypes.ShapeType.Rectangle_:
                    var ptsRectangle = points as Point[] ?? points.ToArray();
                    if (ptsRectangle.Length < 4)
                        throw new Exception("Cannot create rectangle: points array has less than 4 points");
                    return new Quadrangle(ptsRectangle[0], ptsRectangle[1], ptsRectangle[2], ptsRectangle[3]);

                case ShapeTypes.ShapeType.Line_:
                    var ptsLine = points as Point[] ?? points.ToArray();
                    if (ptsLine.Length < 2)
                        throw new Exception("Cannot create line: points array has less than 2 points");
                    return new Line(ptsLine);

                case ShapeTypes.ShapeType.Ellipse_:
                    var ptsEllipse = points as Point[] ?? points.ToArray();
                    if (ptsEllipse.Length < 3)
                        throw new Exception("Cannot create ellipse: points array has less than 3 points");
                    return new Ellipse(ptsEllipse[0], ptsEllipse[1], ptsEllipse[2]);
            }
            throw new Exception($"Unknown shape: {ShapeTypes.name[shape]}");
        }
    }
}