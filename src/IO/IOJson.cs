using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShapeEditor.Shapes;
using System.IO;
using System.Runtime.Serialization.Json;

namespace ShapeEditor.src.IO
{
    class IOJson : IOShapeEditor
    {
        public List<Shape> loadShapes(string filePath)
        {
            List<Shape> list = null;

            FileStream fin = new FileStream(filePath, FileMode.Open);
            DataContractJsonSerializer jSer = new DataContractJsonSerializer(typeof(List<Shape>));
            list = (List<Shape>)jSer.ReadObject(fin);
            return list;
        }

        public int saveShapes(List<Shape> shapes, string filePath)
        {
            FileStream fout = new FileStream(filePath, FileMode.OpenOrCreate);
            DataContractJsonSerializer jSer = new DataContractJsonSerializer(typeof(List<Shape>));
            jSer.WriteObject(fout, shapes);
            return 0;
        }
    }
}
