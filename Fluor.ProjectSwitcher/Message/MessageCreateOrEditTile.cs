using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluor.ProjectSwitcher.Class;
using System.Collections.ObjectModel;

namespace Fluor.ProjectSwitcher.Message
{
    public class MessageCreateOrEditTile
    {
        public object Sender { get; set; }
        public Project SelectedTile { get; set; }
        public ObservableCollection<Project> ProjectsCollection { get; set; }

        public MessageCreateOrEditTile(Project selectedTile, object sender, ObservableCollection<Project> projectsCollection)
        {
            SelectedTile = selectedTile;
            Sender = sender;
            ProjectsCollection = projectsCollection;
        }
    }
}
