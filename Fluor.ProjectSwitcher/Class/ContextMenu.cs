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

        private string _value;
        [XmlAttribute("Value")]
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                EditedValue = _value;
            }
        }

        private string name;
        [XmlAttribute()]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                EditedName = name;
            }
        }

        [XmlIgnore]
        public string EditedValue { get; set; }

        [XmlIgnore]
        public string EditedName { get; set; }

        [XmlIgnore]
        public bool IsNew { get; set; }

        public ContextMenu()
        {
            IsNew = false;
        }

        public void Setup(ContextMenuTypeEnum type, string value, string displayName)
        {
            Type = type;
            Value = value;
            Name = displayName;
        }

        public void Update()
        {
            Name = EditedName;
            Value = EditedValue;
        }

        public void CancelUpdate()
        {
            EditedName = Name;
            EditedValue = Value;
        }
    }
}
