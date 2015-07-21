using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fluor.ProjectSwitcher.Class
{
    public static class Serialization
    {
        public static void Serialize(ProjectSwitcherSettings d, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProjectSwitcherSettings));
            using (TextWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, d);
            }
        }

        public static ProjectSwitcherSettings Deserialize(string filePath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(ProjectSwitcherSettings));
            TextReader reader = new StreamReader(@filePath);
            object obj = deserializer.Deserialize(reader);
            reader.Close();
            return (ProjectSwitcherSettings)obj;
        }
    }
}
