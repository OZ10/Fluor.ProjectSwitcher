using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls;

namespace Fluor.SPPID.ProjectSwitcher.ViewModel
{
    public class ProjectsViewModel :ViewModelBase 
    {
        private ObservableCollection<SPPIDProject> projectsCollection;
        public ObservableCollection<SPPIDProject> ProjectsCollection
        {
            get
            {
                return projectsCollection; 
            }
            set
            {
                projectsCollection = value;
                RaisePropertyChanged("ProjectsCollection");
            }
        }
        //public SPPIDProject SelectedSPPIDProject { get; set; }

        public ProjectsViewModel()
        {
            Messenger.Default.Register<Message.PopulateProjectsMessage>(this, UpdatedProjectsCollection);
        }

        private void UpdatedProjectsCollection(Message.PopulateProjectsMessage populateProjectsMessage)
        {
            ProjectsCollection = populateProjectsMessage.ProjectsCollection;
        }

        public void OpenFolder(MenuItem mi, SPPIDProject sppidProject)
        {
            if (sppidProject != null)
            {
                string argument = null;

                //OPEN WINDOWS EXPLORER
                Process p = new Process();
                p.StartInfo.FileName = "explorer.exe";

                //ADD A DIRECTORY PATH AS AN ARGUMENT
                switch (mi.Name)
                {
                    case "miOpenPIDReferenceData":
                        argument = sppidProject.PIDPath;
                        break;
                    case "miOpenEngineeringManagerPath":
                        argument = sppidProject.SPENGPath;
                        break;
                    default:
                        break;
                }
                p.StartInfo.Arguments = argument;
                p.Start();
            }
        }

        public void ProjectChanged(ListView lv)
        {
            SPPIDProject sppidProject = (SPPIDProject)lv.SelectedItem;

            if (sppidProject != null)
            {
                //SelectedSPPIDProject = sppidProject;
                Messenger.Default.Send(new Message.ChangeSelectedProjectMessage(sppidProject));
            }
        }
    }
}
