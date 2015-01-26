using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluor.ProjectSwitcher.Class
{
    public class ContextMenu
    {
        public enum TypeEnum
        {
            PATH
        };

        public TypeEnum Type { get; set; }
        public string Value { get; set; }
        public string DisplayName { get; set; }

        public ContextMenu(TypeEnum type, string value, string displayName)
        {
            Type = type;
            Value = value;
            DisplayName = displayName;
        }
    }
}
