using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluor.ProjectSwitcher.Message
{
    class M_SettingsHaveBeenChanged
    {
        public bool SettingsHaveChanged { get; set; }
        public M_SettingsHaveBeenChanged(bool settingChanged)
        {
            SettingsHaveChanged = settingChanged;
        }
    }
}
