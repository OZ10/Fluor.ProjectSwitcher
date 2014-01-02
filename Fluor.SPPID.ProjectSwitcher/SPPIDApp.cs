using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluor.SPPID.ProjectSwitcher
{
    class SPPIDApp
    {
        public string AppName { get; set; }
        public string ParentApp { get; set; }
        public string Exe { get; set; }
        public bool IsChecked { get; set; }

        public SPPIDApp()
        {
            
        }
    }
}
