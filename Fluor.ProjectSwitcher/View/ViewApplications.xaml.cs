using Fluor.ProjectSwitcher.Class;
using System.Windows;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher.View
{
    /// <summary>
    /// Interaction logic for ApplicationsUC.xaml
    /// </summary>
    public partial class ViewApplications : UserControl
    {
        public ViewModel.ViewModelApplications vm
        {
            get
            {
                return (ViewModel.ViewModelApplications)DataContext;
            }
        }

        public ViewApplications()
        {
            InitializeComponent();
        }

        private void muApplication_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            Utilities.OpenFolder(mi.CommandParameter.ToString());
        }
    }
}
