//Develop by Mariano Julio Vicario -
//http://compactplugs.codeplex.com/
//http://www.ranu.com.ar (Blog)
//Microsoft Public License (Ms-PL)

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace CompactInjection.Tools
{
    public class XmlSerializerDeserializer
    {
        public static void Serializer<T>(Object objToSerialize, string xmlPath) where T: class
        {
            XmlSerializer s = new XmlSerializer(typeof(T));
            TextWriter w = new StreamWriter(xmlPath);
            s.Serialize(w, objToSerialize);
            w.Close();
            w = null;
        }

        public static T DeSerializer<T>(string xmlPath) where T : class
        {
            T Restval;
            XmlSerializer s = new XmlSerializer(typeof(T));
            TextReader r = new StreamReader(xmlPath);
            Restval = s.Deserialize(r) as T;
            r.Close();
            r = null;
            return Restval;
        }
    }

}
