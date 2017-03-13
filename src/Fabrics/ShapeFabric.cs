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
        public static Shape CreateShape(ShapeTypes.ShapeType shape, IEnumerable<Point> points, ShapeModes.ShapeMode mode)
        {
            switch (shape)
            {
                case ShapeTypes.ShapeType.Triangle_:
                    var ptsTriangle = points as Point[] ?? points.ToArray();
                    if (ptsTriangle.Length < 3)
                        throw new Exception("Cannot create triangle: points array has less than 3 points");
                    return new Triangle(ptsTriangle[0], ptsTriangle[1], ptsTriangle[2]);

                case ShapeTypes.ShapeType.Quadrangle_:
                    var ptsQuadrangle = points as Point[] ?? points.ToArray();
                    if (ptsQuadrangle.Length < 4)
                        throw new Exception("Cannot create quadrangle: points array has less than 4 points");
                    Quadrangle quadrangle = new Quadrangle(ptsQuadrangle[0], ptsQuadrangle[1], ptsQuadrangle[2], ptsQuadrangle[3]);
                    if (!quadrangle.IsConvex() && mode == ShapeModes.ShapeMode.Fixed)
                    {
                        throw new Exception("Wrong quadrangle");
                    }
                    return quadrangle;

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