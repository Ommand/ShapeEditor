using ShapeEditor.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Globalization;
using System.Xml;
using System.Windows.Media;
using ShapeEditor.Fabrics;

namespace ShapeEditor.src.IO
{
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
        const string CLASS = "className";
        const string ELLIPSE = "ellipse";
        const string POLYLINE = "polyline";
        const string POLYGON = "polygon";
        const string CLASS_ATR = "g";
        const string TRANSFORM = "transform";
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

        #region Save methods
        private int writePolyline(XmlWriter writer, Shape shape)
        {
            writer.WriteStartElement(POLYLINE);

            List<Point> points = this.TransformToPixels(shape.GetPoints.ToList<Point>());
            IDrawable2DShape colorShape = (IDrawable2DShape)shape;

            string fill = this.ColorToString(colorShape.FillColor);
            string width = colorShape.BorderWidth.ToString();
            string borderColor = this.ColorToString(colorShape.BorderColor);
            string fillOpacity = ((double)colorShape.FillColor.A / 255 ).ToString();
            string borderColorOpacity = ((double)colorShape.BorderColor.A / 255).ToString();

            fillOpacity = "0.0";

            string spoints = "";
            int N = points.Count - 1;
            for (int i = 0; i < N; i++)
            {
                Point item = points[i];
                spoints += (item.X.ToString() + "," + item.Y.ToString() + " ");
            }
            spoints += (points[N].X.ToString() + "," + points[N].Y.ToString());

            writer.WriteAttributeString(POINTS, spoints);
            writer.WriteAttributeString(FILL, fill);
            writer.WriteAttributeString(FILL_OPACITY, fillOpacity);
            writer.WriteAttributeString(STROKE, fill);
            writer.WriteAttributeString(STROKE_WIDTH, width);
            writer.WriteAttributeString(STROKE_OPACITY, borderColorOpacity);

            writer.WriteAttributeString(CLASS, shape.GetType().Name);

            writer.WriteEndElement(); // />
            return 0;
        }
        private int writePolygon(XmlWriter writer, Shape shape)
        {
            writer.WriteStartElement(POLYGON);

            List<Point> points = this.TransformToPixels(shape.GetPoints.ToList<Point>());
            IDrawable2DShape colorShape = (IDrawable2DShape)shape;

            string fill = this.ColorToString(colorShape.FillColor);
            string width = colorShape.BorderWidth.ToString();
            string borderColor = this.ColorToString(colorShape.BorderColor);
            string fillOpacity = ((double)colorShape.FillColor.A / 255).ToString();
            string borderColorOpacity = ((double)colorShape.BorderColor.A / 255).ToString();

            string spoints = "";
            int N = points.Count - 1;
            for (int i = 0; i < N; i++)
            {
                Point item = points[i];
                spoints += (item.X.ToString() + "," + item.Y.ToString() + " ");
            }
            spoints += (points[N].X.ToString() + "," + points[N].Y.ToString());

            writer.WriteAttributeString(POINTS, spoints);
            writer.WriteAttributeString(FILL, fill);
            writer.WriteAttributeString(FILL_OPACITY, fillOpacity);
            writer.WriteAttributeString(STROKE, borderColor);
            writer.WriteAttributeString(STROKE_WIDTH, width);
            writer.WriteAttributeString(STROKE_OPACITY, borderColorOpacity);

            writer.WriteAttributeString(CLASS, shape.GetType().Name);

            writer.WriteEndElement(); // />
            return 0;
        }
        private int writeEllipse(XmlWriter writer, Shape cshape)
        {
            writer.WriteStartElement(ELLIPSE);
            Ellipse shape = cshape as Ellipse;
            Point c = this.TransformPointToPixel(shape.GetCenter());
            Point Upper = this.TransformPointToPixel(shape.GetUpperPoint());
            Point Right = this.TransformPointToPixel(shape.GetRightPoint());

            Point cOrth = shape.GetCenter();

            string formatDouble = "{0:0.###############}";
            string div = "\"";
            string cx = String.Format(CultureInfo.InvariantCulture, formatDouble, c.X);
            string cy = String.Format(CultureInfo.InvariantCulture, formatDouble, c.Y);
            string rx = String.Format(CultureInfo.InvariantCulture, formatDouble, this.Norm(c, Right));
            string ry = String.Format(CultureInfo.InvariantCulture, formatDouble, this.Norm(c, Upper));

            string borderColor = this.ColorToString(shape.BorderColor);
            string borderWidth = shape.BorderWidth.ToString();
            string fill = this.ColorToString(shape.FillColor);

            string borderColorOpacity = ((double)shape.BorderColor.A / 255).ToString();
            string fillColorOpacity = ((double)shape.FillColor.A / 255).ToString();

            double tangle = (-180 * shape.AngleBeetweenMajorAxisAndPositiveX() / Math.PI);

            string transform = "";
            transform += String.Format(CultureInfo.InvariantCulture, "rotate({0:0.###############}", tangle);
            transform += String.Format(CultureInfo.InvariantCulture, ",{0:0.###############}", c.X);
            transform += String.Format(CultureInfo.InvariantCulture, ",{0:0.###############}", c.Y);
            transform += ")";

            List<Point> points = shape.GetPoints.ToList<Point>();
            string ellipsePoints = "";
            int N = points.Count - 1;
            for (int i = 0; i < N; i++)
            {
                Point item = points[i];
                ellipsePoints += String.Format(CultureInfo.InvariantCulture, "{0:0.###############},{1:0.###############}", item.X, item.Y) + " ";
            }
            ellipsePoints += String.Format(CultureInfo.InvariantCulture, "{0:0.###############},{1:0.###############}", points[N].X, points[N].Y);

            writer.WriteAttributeString(CX, cx);
            writer.WriteAttributeString(CY, cy);
            writer.WriteAttributeString(RX, rx);
            writer.WriteAttributeString(RY, ry);
            writer.WriteAttributeString(TRANSFORM, transform);
            writer.WriteAttributeString(FILL, fill);
            writer.WriteAttributeString(FILL_OPACITY, fillColorOpacity);
            writer.WriteAttributeString(STROKE, borderColor);
            writer.WriteAttributeString(STROKE_WIDTH, borderWidth);
            writer.WriteAttributeString(STROKE_OPACITY, borderColorOpacity);
            writer.WriteAttributeString(POINTS_ORTH, ellipsePoints);

            writer.WriteAttributeString(CLASS, cshape.GetType().Name);

            writer.WriteEndElement(); // />
            return 0;
        }
        #endregion

        #region Load methods
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
                    if (resultParse == false) throw new Exception("Невозможно прочитать значение координаты точки");
                    resultParse = Double.TryParse(split[j + 1], style, culture, out y);
                    if (resultParse == false) throw new Exception("Невозможно прочитать значение координаты точки");

                    listPoints.Add(new Point(x, y));
                }
            }

            Color fillColor = (Color)ColorConverter.ConvertFromString(fill);
            Color borderColor = (Color)ColorConverter.ConvertFromString(stroke);
            double borderWidth;
            bool parseResult = Double.TryParse(strokeWidth, out borderWidth);
            if (parseResult == false) throw new Exception("Невозможно прочитать значение ширины контура");

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

            if (split.Length % 2 == 0)
            {
                int N = split.Length / 2;
                for (int i = 0; i < N; i++)
                {
                    int x;
                    int y;

                    Point point;
                    bool resultParse;
                    int j = 2 * i;
                    resultParse = Int32.TryParse(split[j], out x);
                    resultParse = Int32.TryParse(split[j + 1], out y);

                    point = this.TransformPixelToOrtho(x, y);
                    listPoints.Add(point);
                }
            }
            else throw new Exception("Точки указаны некорректно");

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
        #endregion

        public int saveShapes(List<Shape> shapes, string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.Default))
            {
                XmlWriterSettings ws = new XmlWriterSettings();
                ws.Indent = true;
                using (XmlTextWriter writer = new XmlTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;
                    string xmlns = @"http://www.w3.org/2000/svg";
                    string xmlns_xlink = @"http://www.w3.org/1999/xlink";

                    writer.WriteStartElement("svg");
                    writer.WriteAttributeString("version", "1.1");
                    writer.WriteAttributeString("xmlns", xmlns);
                    writer.WriteAttributeString("xmlns", "xlink", null, xmlns_xlink);
                    writer.WriteAttributeString("xml", "space", null, "preserve");
                    foreach (Shape item in shapes)
                    {
                        if (item is Ellipse) writeEllipse(writer, item);
                        else if (item is Line) writePolyline(writer, item);
                        else if (item is Triangle) writePolygon(writer, item);
                        else if (item is Quadrangle) writePolygon(writer, item);
                    }
                    writer.WriteEndElement(); // </svg>
                }
            }
            return 0;
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
                                        className = reader.GetAttribute(CLASS);

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
                                        className = reader.GetAttribute(CLASS);

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
