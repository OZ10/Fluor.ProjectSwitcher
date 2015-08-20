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
            Messenger.Default.Send(new Message.M_EditTile(newProject, this));
            //Messenger.Default.Send<Project>(newProject);
            //vm.AddNewTile();
        }

        private void tile_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Grid grid = (Grid)sender;
            vm.DisplayContextMenus(grid);
        }

        private void muProject_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            //TODO The binding is not working correctly here; I'm not sure why. For some reason the
            //     The ContextMenu class is bound to the header property of the MenuItem.
            Class.ContextMenu contextMenu = (Class.ContextMenu)mi.Header;

            Utilities.OpenFolder(contextMenu.Value);
        }

        private void btnTileMain_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Project selectedProject = btn.DataContext as Project;

            if (selectedProject != null)
            {
                Messenger.Default.Send(new Message.M_UpdateSelectedTile(selectedProject, this));
            }
            else
            {
                TopApplication selectedApplication = btn.DataContext as TopApplication;
                Messenger.Default.Send(new Message.M_UpdateSelectedTile(selectedApplication, this));
            }
        }

        // Edit button click on tile
        private void btnTileEdit_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            Project selectedProject = btn.DataContext as Project;

            if (selectedProject != null)
            {
                Messenger.Default.Send(new Message.M_EditTile(selectedProject, this));
            }
            else
            {
                TopApplication selectedApplication = btn.DataContext as TopApplication;

                if (selectedApplication != null)
                {
                    Messenger.Default.Send(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayEditApplicationsTab));
                }
            }
        }
    }
}
