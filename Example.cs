using System;

namespace ShapeEditor
{
    struct Point { }

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
