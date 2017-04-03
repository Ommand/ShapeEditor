﻿using ShapeEditor.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapeEditor.src.IO
{
    class IOSvg: IOShapeEditor
    {
        #region SaveMethods
        private int saveEllipse()
        {
            return 0;
        }

        private int saveLine()
        {
            return 0;
        }

        private int saveQuadrangle()
        {
            return 0;
        }

        private int saveTriangle()
        {
            return 0;
        }

        #endregion

        #region LoadMethods
        private int loadEllipse()
        {
            return 0;
        }

        private int loadLine()
        {
            return 0;
        }

        private int loadQuadrangle()
        {
            return 0;
        }

        private int loadTriangle()
        {
            return 0;
        }

        #endregion

        #region Methods

        private string getFormatValueSvg(string str)
        {
            string div = "\"";
            return (div + str + div);
        }

        private string createLineString(Line line)
        {
            string result = "<g class=\"Line\">\n\t";
            string div = "\"";
            List<Point> points = (List<Point>)line.GetPoints;

            string fill = div + line.FillColor.ToString() + div;
            string width = div + line.BorderWidth.ToString() + div;
            string borderColor = div + line.BorderColor.ToString() + div;

            result += "<polyline points=";
            foreach(Point item in points)
            {
                result += div + item.X.ToString() + "," + item.Y.ToString() + " ";
            }
            result += String.Format(div + "fill={0} stroke={1} stroke-width={2}/>", fill, borderColor, width);
            result += ("\n</g>\n");
            return result;
        }

        private string createTriangleString(Triangle triangle)
        {
            string result = "<g class=\"Triangle\">\n\t";
            string div = "\"";
            List<Point> points = (List<Point>)triangle.GetPoints;

            string fill = div + triangle.FillColor.ToString() + div;
            string width = div + triangle.BorderWidth.ToString() + div;
            string borderColor = div + triangle.BorderColor.ToString() + div;

            result += "<polygon points=";
            foreach (Point item in points)
            {
                result += div + item.X.ToString() + "," + item.Y.ToString() + " ";
            }
            result += String.Format(div + "fill={0} stroke={1} stroke-width={2}/>", fill, borderColor, width);
            result += ("\n</g>\n");
            return result;
        }

        private string createQuadrangleString(Quadrangle quadrangle)
        {
            string result = "<g class=\"Quadrangle\">\n\t";
            string div = "\"";
            List<Point> points = (List<Point>)quadrangle.GetPoints;

            string fill = div + quadrangle.FillColor.ToString() + div;
            string width = div + quadrangle.BorderWidth.ToString() + div;
            string borderColor = div + quadrangle.BorderColor.ToString() + div;

            result += "<polygon points=";
            foreach (Point item in points)
            {
                result += div + item.X.ToString() + "," + item.Y.ToString() + " ";
            }
            result += String.Format(div + "fill={0} stroke={1} stroke-width={2}/>", fill, borderColor, width);
            result += ("\n</g>\n");
            return result;
        }
        private string createEllipseString(Ellipse shape)
        {
            Point c = shape.GetCenter();
            
            string cx = getFormatValueSvg(c.X.ToString());
            string cy = getFormatValueSvg(c.Y.ToString());
            string rx = getFormatValueSvg(shape.GetSemiMajorAxis().ToString());
            string ry = getFormatValueSvg(shape.GetSemiMinorAxis().ToString());

            string borderColor = getFormatValueSvg(shape.BorderColor.ToString());
            string borderWidth = getFormatValueSvg(shape.BorderWidth.ToString());
            string fill = getFormatValueSvg(shape.FillColor.ToString());

            string tangle = (-180 * shape.AngleBeetweenMajorAxisAndPositiveX() / Math.PI).ToString();
            string transform = String.Format("\"rotate({0}, {1}, {2})\"", tangle, c.X.ToString(), c.Y.ToString());

            string result = "<g class=\"Ellipse\">\n\t";
            result += String.Format("<ellipse> cx={0} cy={1} rx={2} ry={3} fill={4} stroke={5} stroke-width={6} transform={7}", 
                cx, cy, rx, ry, fill, borderColor, borderWidth, transform);
            result += ("\n</g>\n");
            return result;
        }
        public int saveShapes(List<Shape> shapes, string filePath)
        {
            FileStream fout = new FileStream(filePath, FileMode.Create);
            string head = "<!DOCTYPE svg PUBLIC \" -//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">" +
                "< svg version = \"1.1\" xmlns = \"http://www.w3.org/2000/svg\"" +
                " xmlns: xlink = \"http://www.w3.org/1999/xlink\" xml: space = \"preserve\" >\n\t";

            using (StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(head);
                foreach (Shape item in shapes)
                {
                    string shapeString = "";

                    if (item is Ellipse) shapeString = createEllipseString((Ellipse)item);
                    else if (item is Line) shapeString = createLineString((Line)item);
                    else if (item is Triangle) shapeString = createTriangleString((Triangle)item);
                    else if (item is Quadrangle) shapeString = createQuadrangleString((Quadrangle)item);

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
