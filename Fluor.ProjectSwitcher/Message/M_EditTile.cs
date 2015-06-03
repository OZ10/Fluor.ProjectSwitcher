using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluor.ProjectSwitcher.Class;
using System.Collections.ObjectModel;

namespace Fluor.ProjectSwitcher.Message
{
    public class M_EditTile
    {
        public object Sender { get; set; }
        public Project SelectedTile { get; set; }

        public M_EditTile(Project selectedTile, object sender)
        {
            SelectedTile = selectedTile;
            Sender = sender;
        }
    }
}
