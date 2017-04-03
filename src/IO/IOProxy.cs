using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShapeEditor.Shapes;
using System.Text.RegularExpressions;

namespace ShapeEditor.src.IO
{
    public class IOProxy : IOShapeEditor
    {
        const string SVG_FORMAT = @"\w*\.svg$";     // *.svg
        const string XML_FORMAT = @"\w*\.xml$";     // *.xml
        const string JSON_FORMAT = @"\w*\.json$";   // *.json

        private IOShapeEditor ioShape;
        private bool checkFormat(string filePath)
        {
            if(Regex.IsMatch(filePath, SVG_FORMAT, RegexOptions.Compiled | RegexOptions.IgnoreCase))
            {
                ioShape = new IOSvg();
                return true;
            }
            else if(Regex.IsMatch(filePath, XML_FORMAT, RegexOptions.Compiled | RegexOptions.IgnoreCase))
            {
                ioShape = new IOSvg();
                return true;
            }
            else if(Regex.IsMatch(filePath, JSON_FORMAT, RegexOptions.Compiled | RegexOptions.IgnoreCase))
            {
                ioShape = new IOJson();
                return true;
            }
            return false;
        }

        List<Shape> IOShapeEditor.loadShapes(string filePath)
        {
            List<Shape> result = null;
            if (checkFormat(filePath))
            {
                result = ioShape.loadShapes(filePath);
            }
            return result;
        }
        int IOShapeEditor.saveShapes(List<Shape> shapes, string filePath)
        {
            if (checkFormat(filePath))
            {
                ioShape.saveShapes(shapes, filePath);
            }
            return 0;
        }
    }
}