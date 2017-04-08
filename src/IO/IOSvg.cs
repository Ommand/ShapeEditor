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
using System.Xml;
using System.Windows.Media;
using ShapeEditor.Fabrics;

namespace ShapeEditor.src.IO
{
    struct BuilderParametrs
    {
        string points;
        string fill;
        string fillOpacity;

        string stroke;
        string strokeOpacity;
        string strokeWidth;

        string rotate_angle;
        string rotate_x0;
        string rotate_y0;

        string cx, cy, rx, ry;
        string className;
    }

    class IOSvg: IOData, IOShapeEditor
    {
        public IOSvg(Func<int, int, Point> _TransformPixelToOrtho, Func<double, double, Point> _TransformOrthoToPixel) 
            : base(_TransformPixelToOrtho, _TransformOrthoToPixel)
        {
        }

        #region Consts
        const string FILL = "fill";
        const string STROKE = "stroke";
        const string STROKE_WIDTH = "stroke-width";
        const string FILL_OPACITY = "fill-opacity";
        const string STROKE_OPACITY = "stroke-opacity";
        const string CX = "cx";
        const string CY = "cy";
        const string RX = "rx";
        const string RY = "ry";
        const string POINTS = "points";
        const string CLASS = "class";
        const string ELLIPSE = "ellipse";
        const string POLYLINE = "polyline";
        const string POLYGON = "polygon";
        const string CLASS_ATR = "g";

        const string FOCUS1_ORTH = "focus1-orth";
        const string FOCUS2_ORTH = "focus2-orth";
        const string POINTS_ORTH = "points-orth";
        #endregion

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
            List<Point> points = this.TransformToPixels(shape.GetPoints.ToList<Point>());

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
            List<Point> linePoints = line.GetPoints.ToList<Point>(); 
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
            string fillc = this.ColorToString(shape.FillColor);

            Point c = this.TransformPointToPixel(shape.GetCenter());
            Point upper = this.TransformPointToPixel(shape.GetUpperPoint());
            Point Right = this.TransformPointToPixel(shape.GetRightPoint());

            Point cOrth = shape.GetCenter();

            string formatDouble = "{0:0.0}";
            string div = "\"";
            string cx = div + String.Format(CultureInfo.InvariantCulture, formatDouble, c.X) + div;
            string cy = div + String.Format(CultureInfo.InvariantCulture, formatDouble, c.Y) + div;
            string rx = div + String.Format(CultureInfo.InvariantCulture, formatDouble, this.Norm(c, Right)) + div;
            string ry = div + String.Format(CultureInfo.InvariantCulture, formatDouble, this.Norm(c, upper)) + div;

            string borderColor = getFormatValueSvg(this.ColorToString(shape.BorderColor));
            string borderWidth = getFormatValueSvg(shape.BorderWidth.ToString());
            string fill = getFormatValueSvg(this.ColorToString(shape.FillColor));

            string borderColorOpacity = div + shape.BorderColor.A.ToString() + div;
            string fillColorOpacity = div + shape.FillColor.A.ToString() + div;

            double tangle = (-180 * shape.AngleBeetweenMajorAxisAndPositiveX() / Math.PI);

            string transform = "";
            transform += String.Format(CultureInfo.InvariantCulture, "\"rotate({0:0.###############}", tangle);
            transform += String.Format(CultureInfo.InvariantCulture, ",{0:0.###############}", c.X);
            transform += String.Format(CultureInfo.InvariantCulture, ",{0:0.###############}", c.Y);
            transform += ")" + div;

            string result = "<g class=\"Ellipse\">\n\t";
            result += String.Format("<ellipse cx={0} cy={1} rx={2} ry={3} fill={4} stroke={5} stroke-width={6} transform={7} stroke-opacity={8} fill-opacity={9}", 
                cx, cy, rx, ry, fill, borderColor, borderWidth, transform, borderColorOpacity, fillColorOpacity);

            List<Point> points = shape.GetPoints.ToList<Point>();
            string ellipsePoints = "";
            int N = points.Count - 1;
            for (int i = 0; i < N; i++)
            {
                Point item = points[i];
                ellipsePoints += String.Format(CultureInfo.InvariantCulture, "{0:0.###############},{1:0.###############}", item.X, item.Y) + " ";
            }
            ellipsePoints += String.Format(CultureInfo.InvariantCulture, "{0:0.#},{1:0.#}", points[N].X, points[N].Y);
            result += String.Format(" {0}=\"{1}\"", POINTS_ORTH, ellipsePoints);

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

        private Shape buildEllipse(string className, string cx, string cy, string rx, string ry, string fill, string fillOpacity, string stroke, string strokeWidth, string strokeOpacity)
        {
            Ellipse shape;
            List<Point> listPoints;

            return null;
        }
        private Shape buildEllipseWithOrth(string className, string pointsOrth, string fill, string fillOpacity, string stroke, string strokeWidth, string strokeOpacity)
        {
            char[] sep = new char[2];
            sep[0] = ',';
            sep[1] = ' ';
            string[] split = pointsOrth.Split(sep);
            List<Point> listPoints = new List<Point>();
            NumberStyles style = NumberStyles.Number | NumberStyles.AllowDecimalPoint;
            CultureInfo culture = CultureInfo.InvariantCulture;

            if (split.Length % 2 == 0)
            {
                int N = split.Length / 2;
                for (int i = 0; i < N; i++)
                {
                    double x;
                    double y;

                    Point point;
                    bool resultParse;
                    int j = 2 * i;
                    resultParse = Double.TryParse(split[j], style, culture, out x);
                    resultParse = Double.TryParse(split[j + 1], style, culture, out y);

                    listPoints.Add(new Point(x, y));
                }
            }

            Color fillColor = (Color)ColorConverter.ConvertFromString(fill);
            Color borderColor = (Color)ColorConverter.ConvertFromString(stroke);
            double borderWidth;
            bool parseResult = Double.TryParse(strokeWidth, out borderWidth);
            ShapeTypes.ShapeType shapeType;
            switch (className)
            {
                case "Ellipse":
                    shapeType = ShapeTypes.ShapeType.Ellipse_;
                    break;
                case "Triangle":
                    shapeType = ShapeTypes.ShapeType.Triangle_;
                    break;
                case "Quadrangle":
                    shapeType = ShapeTypes.ShapeType.Quadrangle_;
                    break;
                case "Line":
                    shapeType = ShapeTypes.ShapeType.Line_;
                    break;
                default:
                    throw new Exception("Указан неизвестный тип фигуры");
            }

            Shape shape = ShapeFabric.CreateShape(shapeType, listPoints, ShapeModes.ShapeMode.Fixed);
            IDrawable2DShape colorShape = shape as IDrawable2DShape;
            colorShape.BorderColor = borderColor;
            colorShape.FillColor = fillColor;
            colorShape.BorderWidth = (float)borderWidth;

            return shape;
        }
        private Shape buildShape(string className, string points, string fill, string fillOpacity, string stroke, string strokeWidth, string strokeOpacity)
        {
            char[] sep = new char[2];
            sep[0] = ',';
            sep[1] = ' ';
            string[] split = points.Split(sep);
            List<Point> listPoints = new List<Point>();

            if(split.Length % 2 == 0)
            {
                int N = split.Length / 2;
                for(int i = 0; i < N; i++)
                {
                    int x;
                    int y;

                    Point point;
                    bool resultParse;
                    int j = 2 * i;
                    resultParse = Int32.TryParse(split[j], out x);
                    resultParse = Int32.TryParse(split[j+1], out y);

                    point = this.TransformPixelToOrtho(x, y);
                    listPoints.Add(point);
                }
            }

            Color fillColor = (Color)ColorConverter.ConvertFromString(fill);
            Color borderColor = (Color)ColorConverter.ConvertFromString(stroke);
            double borderWidth;
            bool parseResult = Double.TryParse(strokeWidth, out borderWidth);
            ShapeTypes.ShapeType shapeType;
            switch(className)
            {
                case "Ellipse":
                    shapeType = ShapeTypes.ShapeType.Ellipse_;
                    break;
                case "Triangle":
                    shapeType = ShapeTypes.ShapeType.Triangle_;
                    break;
                case "Quadrangle":
                    shapeType = ShapeTypes.ShapeType.Quadrangle_;
                    break;
                case "Line":
                    shapeType = ShapeTypes.ShapeType.Line_;
                    break;
                default:
                    throw new Exception("Указан неизвестный тип фигуры");
            }

            Shape shape = ShapeFabric.CreateShape(shapeType, listPoints, ShapeModes.ShapeMode.Fixed);
            IDrawable2DShape colorShape = shape as IDrawable2DShape;
            colorShape.BorderColor = borderColor;
            colorShape.FillColor = fillColor;
            colorShape.BorderWidth = (float)borderWidth;

            return shape;
        }
        private List<Point> parseStringPoints(string points)
        {
            List<Point> result = new List<Point>();

            return result;
        }
        public List<Shape> loadShapes(string filePath)
        {
            List<Shape> listShapes = new List<Shape>();
            using (StreamReader fin = new StreamReader(filePath, System.Text.Encoding.Default))
            {             
                XmlReaderSettings XmlSettings = new XmlReaderSettings();
                XmlSettings.DtdProcessing = DtdProcessing.Parse;
                using (XmlReader reader = XmlReader.Create(fin, XmlSettings))
                {
                    string points;
                    string fill;
                    string fillOpacity;

                    string stroke;
                    string strokeOpacity;
                    string strokeWidth;

                    string rotate_angle;
                    string rotate_x0;
                    string rotate_y0;

                    string pointsOrth;

                    string cx, cy, rx, ry;
                    string className = "";
                                        
                    Shape shape;

                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                {
                                    string nodeName = reader.Name;
                                    if (nodeName == CLASS_ATR)
                                    {
                                        className = reader.GetAttribute(CLASS);
                                    }
                                    else if (nodeName == POLYGON || nodeName == POLYLINE)
                                    {
                                        points = reader.GetAttribute(POINTS);
                                        fill = reader.GetAttribute(FILL);
                                        fillOpacity = reader.GetAttribute(FILL_OPACITY);
                                        stroke = reader.GetAttribute(STROKE);
                                        strokeWidth = reader.GetAttribute(STROKE_WIDTH);
                                        strokeOpacity = reader.GetAttribute(STROKE_OPACITY);
                                        shape = buildShape(className, points, fill, fillOpacity, stroke, strokeWidth, strokeOpacity);
                                        if (shape != null) listShapes.Add(shape);
                                    }
                                    else if (nodeName == ELLIPSE)
                                    {
                                        cx = reader.GetAttribute(CX);
                                        cy = reader.GetAttribute(CY);
                                        rx = reader.GetAttribute(RX);
                                        ry = reader.GetAttribute(RY);

                                        pointsOrth = reader.GetAttribute(POINTS_ORTH);

                                        fill = reader.GetAttribute(FILL);
                                        fillOpacity = reader.GetAttribute(FILL_OPACITY);
                                        stroke = reader.GetAttribute(STROKE);
                                        strokeWidth = reader.GetAttribute(STROKE_WIDTH);
                                        strokeOpacity = reader.GetAttribute(STROKE_OPACITY);
                                        //shape = buildEllipse(className, cx, cy, rx, ry, fill, fillOpacity, stroke, strokeWidth, strokeOpacity);
                                        shape = buildEllipseWithOrth(className, pointsOrth, fill, fillOpacity, stroke, strokeWidth, strokeOpacity);
                                        if (shape != null) listShapes.Add(shape);
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            return listShapes;
        }

        #endregion
    }
}
