using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fluor.ProjectSwitcher.Class
{
    public class ContextMenu
    {
        public enum ContextMenuTypeEnum
        {
            PATH
        };

        [XmlAttribute("ContextMenuType")]
        public ContextMenuTypeEnum Type { get; set; }

        [XmlAttribute("Value")]
        public string Value { get; set; }

        [XmlAttribute()]
        public string DisplayName { get; set; }

        public ContextMenu()
        {
            
        }

        public void Setup(ContextMenuTypeEnum type, string value, string displayName)
        {
            Type = type;
            Value = value;
            DisplayName = displayName;
        }
    }
}
