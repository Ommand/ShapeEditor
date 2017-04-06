using ShapeEditor.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;

namespace ShapeEditor.src.IO
{
    class IOSvg: IOData, IOShapeEditor
    {
        public IOSvg(Func<int, int, Point> _TransformPixelToOrtho, Func<double, double, Point> _TransformOrthoToPixel) 
            : base(_TransformPixelToOrtho, _TransformOrthoToPixel)
        {
        }

        #region Methods

        private List<Point> TransformToPixels(List<Point> list)
        {
            List<Point> result = new List<Point>();
            foreach(Point item in list)
            {
                result.Add(this.TransformPointToPixel(item));
            }
            return result;
        }
        private List<Point> TransformToOrthos(List<Point> list)
        {
            List<Point> result = new List<Point>();
            foreach (Point item in list)
            {
                result.Add(this.TransformPointToOrtho(item));
            }
            return result;
        }
        private string getFormatValueSvg(string str)
        {
            string div = "\"";
            return (div + str + div);
        }
        private string createPolygonString(Shape shape)
        {
            string result = "<g class=\"" + shape.GetType().Name + "\">\n\t";
            string div = "\"";
            List<Point> points = this.TransformToPixels((List<Point>)shape.GetPoints);

            IDrawable2DShape colorShape = (IDrawable2DShape)shape;

            string fill = getFormatValueSvg(this.ColorToString(colorShape.FillColor));
            string width = getFormatValueSvg(colorShape.BorderWidth.ToString());
            string borderColor = getFormatValueSvg(this.ColorToString(colorShape.BorderColor));
            string fillOpacity = getFormatValueSvg(colorShape.FillColor.A.ToString());
            string borderColorOpacity = getFormatValueSvg(colorShape.BorderColor.A.ToString());

            result += "<polygon points=\"";
            int N = points.Count-1;
            for(int i=0;i<N;i++)
            {
                Point item = points[i];
                result += (item.X.ToString() + "," + item.Y.ToString() + " ");
            }
            result += (points[N].X.ToString() + "," + points[N].Y.ToString());
            result += String.Format(div + " fill={0} stroke={1} stroke-width={2} fill-opacity={3} stroke-opacity={4}/>", 
                fill, borderColor, width, fillOpacity, borderColorOpacity);
            result += ("\n</g>\n");
            return result;
        }
        private string createLineString(Line line)
        {
            string result = "<g class=\"" + line.GetType().Name + "\">\n\t";
            string div = "\"";
            Point[] linePoints = (Point[])line.GetPoints; 
            List<Point> points = this.TransformToPixels(linePoints.ToList<Point>());

            string fill = getFormatValueSvg(this.ColorToString(line.FillColor));
            string width = getFormatValueSvg(line.BorderWidth.ToString());
            string borderColor = getFormatValueSvg(this.ColorToString(line.BorderColor));

            string fillOpacity = "\"0\"";// getFormatValueSvg(line.FillColor.A.ToString());
            string borderColorOpacity = getFormatValueSvg(line.BorderColor.A.ToString());

            result += "<polyline points=\"";
            int N = points.Count - 1;
            for (int i = 0; i < N; i++)
            {
                Point item = points[i];
                result += (item.X.ToString() + "," + item.Y.ToString() + " ");
            }
            result += (points[N].X.ToString() + "," + points[N].Y.ToString());
            result += String.Format(div + " fill={0} stroke={1} stroke-width={2} fill-opacity={3} stroke-opacity={4}/>",
                fill, fill, width, fillOpacity, borderColorOpacity);
          
            result += ("\n</g>\n");
            return result;
        }
        private string createEllipseString(Ellipse shape)
        {
            Point orth0 = new Point(0, 0);
            Point orth1 = new Point(1, 1);
            Point pixel0 = this.TransformPointToPixel(orth0);
            Point pixel1 = this.TransformPointToPixel(orth1);

            double scaleX = (orth1.X - orth0.X) / (pixel1.X - pixel0.X);
            double scaleY = (orth1.Y - orth0.Y) / (pixel1.Y - pixel0.Y);

            double shiftX = pixel0.X - scaleX * orth0.X;
            double shiftY = pixel0.Y - scaleY * orth0.Y;

            string fillc = this.ColorToString(shape.FillColor);

            Point c = this.TransformPointToPixel(shape.GetCenter());
            string formatDouble = "{0:0.0}";
            string div = "\"";
            string cx = div + String.Format(CultureInfo.InvariantCulture, formatDouble, c.X) + div;
            string cy = div + String.Format(CultureInfo.InvariantCulture, formatDouble, c.Y) + div;
            string rx = div + String.Format(CultureInfo.InvariantCulture, formatDouble, scaleX*shape.GetSemiMajorAxis() + shiftX) + div;
            string ry = div + String.Format(CultureInfo.InvariantCulture, formatDouble, scaleY*shape.GetSemiMinorAxis() + shiftY) + div;

            string borderColor = getFormatValueSvg(this.ColorToString(shape.BorderColor));
            string borderWidth = getFormatValueSvg(shape.BorderWidth.ToString());
            string fill = getFormatValueSvg(this.ColorToString(shape.FillColor));

            string borderColorOpacity = div + shape.BorderColor.A.ToString() + div;
            string fillColorOpacity = div + shape.FillColor.A.ToString() + div;

            double tangle = (-180 * shape.AngleBeetweenMajorAxisAndPositiveX() / Math.PI);

            string transform = "";
            transform += String.Format(CultureInfo.InvariantCulture, "\"rotate({0:0.0}", tangle);
            transform += String.Format(CultureInfo.InvariantCulture, ",{0:0.0}", c.X);
            transform += String.Format(CultureInfo.InvariantCulture, ",{0:0.0}", c.Y);
            transform += ")" + div;

            string result = "<g class=\"Ellipse\">\n\t";
            result += String.Format("<ellipse cx={0} cy={1} rx={2} ry={3} fill={4} stroke={5} stroke-width={6} transform={7} stroke-opacity={8} fill-opacity={9}", 
                cx, cy, rx, ry, fill, borderColor, borderWidth, transform, borderColorOpacity, fillColorOpacity);
            result += "/>";
            result += ("\n</g>\n");
            return result;
        }
        public int saveShapes(List<Shape> shapes, string filePath)
        {
            string head = "<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">";
            head += "\n<svg version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xml:space=\"preserve\">";
            using (StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(head);
                foreach (Shape item in shapes)
                {
                    string shapeString = "";

                    if (item is Ellipse) shapeString = createEllipseString((Ellipse)item);
                    else if (item is Line) shapeString = createLineString((Line)item);
                    else if (item is Triangle) shapeString = createPolygonString((Triangle)item);
                    else if (item is Quadrangle) shapeString = createPolygonString((Quadrangle)item);

                    sw.WriteLine(shapeString);
                }
                sw.WriteLine("</svg>");
            }
            return 0;
        }
        public List<Shape> loadShapes(string filePath)
        {
            return null;
        }

        #endregion
    }
}
