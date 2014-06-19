using System.Windows;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher.View
{
    /// <summary>
    /// Interaction logic for ApplicationsUC.xaml
    /// </summary>
    public partial class ApplicationsUC : UserControl
    {
        public ViewModel.ViewModelApplications vm
        {
            get
            {
                return (ViewModel.ViewModelApplications)DataContext;
            }
        }

        public ApplicationsUC()
        {
            InitializeComponent();
        }

        private void muApplication_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            Class.Utilities.OpenFolder(mi.CommandParameter.ToString());
        }

        private void chkApplication_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;

            vm.DisplayContextMenus(cb.Content.ToString());
        }
    }
}
