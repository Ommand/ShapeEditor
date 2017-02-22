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
        public static IShape CreateShape(ShapeTypes.ShapeType shape, IEnumerable<Point> points)
        {
            switch (shape)
            {
                case ShapeTypes.ShapeType.Triangle_:
                    var pts = points as Point[] ?? points.ToArray();
                    if (pts.Length < 3)
                        throw new Exception("Cannot create triangle: points array has less than 3 points");
                    return new Triangle(pts[0], pts[1], pts[2]);
            }
            throw new Exception($"Unknown shape: {ShapeTypes.name[shape]}");
        }
    }
}