using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fluor.ProjectSwitcher.Message
{
    public class M_LoadFromSettings
    {
        public enum SettingLoadType
        {
            Project,
            Application
        }

        public SettingLoadType LoadType { get; set; }

        public XElement XmlSettings;

        public M_LoadFromSettings(SettingLoadType loadType, XElement xmlSettings)
        {
            LoadType = loadType;
            XmlSettings = xmlSettings;
        }
    }
}
