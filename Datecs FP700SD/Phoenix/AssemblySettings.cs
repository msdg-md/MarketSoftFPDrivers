using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Phoenix
{
    public static class AssemblySettings
    {
        public static readonly string ScaleDBConnection = "ScaleConnectionString";

        private static string PathConfig { get; set; } = Assembly.GetCallingAssembly().Location + ".config";

        public static IDictionary<string, string> ConfigurationInstance = new Dictionary<string, string>();

        public static void loadConfiguration()
        {
            ConfigurationInstance = new Dictionary<string, string>();
            try
            {
                if (!File.Exists(PathConfig))
                {
                    throw new FileNotFoundException(string.Format("Файл {0} не найден",
                                                                  Environment.NewLine, PathConfig));
                }
                XDocument doc = null;
                doc = XDocument.Load(PathConfig);
                if (doc.Root != null)
                {
                    var configList = doc.Root.Elements().Where(x => x.Name == "key");
                    foreach (var xElement in configList)
                    {
                        string key = xElement.Attribute("name") != null ? xElement.Attribute("name").Value : "";
                        string content = xElement.Value;
                        ConfigurationInstance.Add(key, content);
                    }
                }
                doc = null;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in loadConfiguration", ex); ;
            }
        }
    }
}
