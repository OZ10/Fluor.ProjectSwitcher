using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluor.ProjectSwitcher.Class;
using System.Collections.ObjectModel;

namespace Fluor.ProjectSwitcher.Message
{
    public class M_AddOrEditTile
    {
        public object Sender { get; set; }
        public Project SelectedTile { get; set; }

        public M_AddOrEditTile(Project selectedTile, object sender)
        {
            SelectedTile = selectedTile;
            Sender = sender;
        }
    }
}
