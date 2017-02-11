using System;
using ShapeEditor.Fabrics;
using ShapeEditor.Shapes;

namespace ShapeEditor
{

    class SuperShapeFabric : ShapeFabric
    {
        protected override IShape InternalCreateShape(string json)
        {
            throw new NotImplementedException();
        }

        public static void SetCurrent()
        {
            Current = new SuperShapeFabric();
        }
    }
}
