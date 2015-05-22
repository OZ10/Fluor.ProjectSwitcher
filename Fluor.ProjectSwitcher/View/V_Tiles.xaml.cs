using Fluor.ProjectSwitcher.Class;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher.View
{
    /// <summary>
    /// Interaction logic for ProjectsUC.xaml
    /// </summary>
    public partial class V_Tiles : UserControl
    {
        public ViewModel.VM_Tiles vm
        {
            get
            {
                return (ViewModel.VM_Tiles)DataContext;
            }
        }

        public V_Tiles()
        {
            InitializeComponent();
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            Project newProject = new Project();
            newProject.Setup("Project Name", null, "", true, null, true);
            Messenger.Default.Send<Project>(newProject);
            //vm.AddNewTile();
        }
    }
}
