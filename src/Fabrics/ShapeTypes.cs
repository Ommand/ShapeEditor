using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeEditor.Fabrics
{
    public static class ShapeTypes
    {
        public enum ShapeType { Line_, Triangle_, Ellipse_, Rectangle_};
        public static Dictionary<ShapeType, string> name = new Dictionary<ShapeType, string>()
        {
          { ShapeType.Line_, "Line"},
          { ShapeType.Triangle_, "Triangle"},
          { ShapeType.Ellipse_, "Ellipse"},
          { ShapeType.Rectangle_, "Rectangle"}
        };
    }
}
