using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeEditor.Fabrics
{
    public static class PointPlaces
    {
        public enum PointPlace { LeftEdge, RightEdge, LowEdge, UpEdge,
                                 LeftUpCorner, RightUpCorner, LeftLowCorner, RightLowCorner };
    }
}
