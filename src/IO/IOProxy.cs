using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShapeEditor.Shapes;
using System.Text.RegularExpressions;
using System.Windows;
using ShapeEditor.Utils;

namespace ShapeEditor.src.IO
{
    public class IOProxy : IOData, IOShapeEditor
    {
        const string SVG_FORMAT = @"\w*\.svg$";     // *.svg
        const string XML_FORMAT = @"\w*\.xml$";     // *.xml
        const string JSON_FORMAT = @"\w*\.json$";   // *.json

        private IOShapeEditor ioShape;

        public IOProxy(GraphicsController _graphicsController, Func<int, int, Point> _TransformPixelToOrtho, Func<double, double, Point> _TransformOrthoToPixel): 
            base(_graphicsController, _TransformPixelToOrtho, _TransformOrthoToPixel)
        {
            
        }
        private bool checkFormat(string filePath)
        {
            if(Regex.IsMatch(filePath, SVG_FORMAT, RegexOptions.Compiled | RegexOptions.IgnoreCase))
            {
                ioShape = new IOSvg(this.graphicsController, this.TransformPixelToOrtho, this.TransformOrthoToPixel);
                return true;
            }
            else if(Regex.IsMatch(filePath, XML_FORMAT, RegexOptions.Compiled | RegexOptions.IgnoreCase))
            {
                ioShape = new IOSvg(this.graphicsController, this.TransformPixelToOrtho, this.TransformOrthoToPixel);
                return true;
            }
            else if(Regex.IsMatch(filePath, JSON_FORMAT, RegexOptions.Compiled | RegexOptions.IgnoreCase))
            {
                ioShape = new IOJson();
                return true;
            }
            return false;
        }

        public List<Shape> loadShapes(string filePath)
        {
            List<Shape> result = null;
            if (checkFormat(filePath))
            {
                result = ioShape.loadShapes(filePath);
            }
            else throw new Exception("Указанный файл имеет недопустимое расширение");
            return result;
        }
        public int saveShapes(List<Shape> shapes, string filePath)
        {
            if(shapes == null) throw new Exception("Список фигур пуст");
            if (checkFormat(filePath))
            {
                ioShape.saveShapes(shapes, filePath);
            }
            else throw new Exception("Указанный файл имеет недопустимое расширение");
            return 0;
        }
    }
}