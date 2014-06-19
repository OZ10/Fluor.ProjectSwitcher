using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher.View
{
    /// <summary>
    /// Interaction logic for ProjectsUC.xaml
    /// </summary>
    public partial class ProjectsUC : UserControl
    {
        public ViewModel.ViewModelProjects vm
        {
            get
            {
                return (ViewModel.ViewModelProjects)DataContext;
            }
        }

        public ProjectsUC()
        {
            InitializeComponent();
            Messenger.Default.Register<Message.MessageChangeSelectedProject>(this, ChangeSelectedProject);
        }

        private void ChangeSelectedProject(Message.MessageChangeSelectedProject changeSelectedProjectMessage)
        {
            //lstProjects.SelectedItem = changeSelectedProjectMessage.SelectedProject;
        }

        private void tvProjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tv = (TreeView)sender;
            vm.ProjectChanged(tv);
        }

        private void lblProject_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Label l = (Label)sender;
            vm.DisplayContextMenus(l.Content.ToString());
        }

        private void muProject_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

             Class.Utilities.OpenFolder(mi.CommandParameter.ToString());
        }
    }
}
