using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluor.ProjectSwitcher.Class;

namespace Fluor.ProjectSwitcher.Message
{
    public class MessageSaveChangesToTile
    {
        public object Sender { get; set; }
        public Project SelectedTile { get; set; }

        public MessageSaveChangesToTile(Project selectedTile, object sender)
        {
            SelectedTile = selectedTile;
            Sender = sender;
        }
    }
}
