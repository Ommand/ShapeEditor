using ShapeEditor.Shapes;

namespace ShapeEditor.Fabrics
{
    abstract class ShapeFabric
    {
        protected static ShapeFabric Current { get; set; }
        protected abstract IShape InternalCreateShape(string json);
        public static IShape CreateShape(string json)
        {
            return Current.InternalCreateShape(json);
        }
    }
}