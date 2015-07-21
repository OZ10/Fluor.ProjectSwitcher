using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluor.ProjectSwitcher.Message
{
    public class M_ChangeView
    {
        public enum ViewToSelect
        {
            DisplayTilesTab,
            DisplayApplicationsTab,
            DisplayAddNewTab,
            DisplayEditApplicationsTab,
        }

        public ViewToSelect View { get; set; }

        public M_ChangeView(ViewToSelect viewToSelect)
        {
            View = viewToSelect;
        }
    }
}
