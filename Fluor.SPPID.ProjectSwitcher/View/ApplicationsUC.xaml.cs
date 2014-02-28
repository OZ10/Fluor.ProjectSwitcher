using System.Windows;
using System.Windows.Controls;

namespace Fluor.SPPID.ProjectSwitcher.View
{
    /// <summary>
    /// Interaction logic for ApplicationsUC.xaml
    /// </summary>
    public partial class ApplicationsUC : UserControl
    {
        public ViewModel.ApplicationsViewModel vm
        {
            get
            {
                return (ViewModel.ApplicationsViewModel)DataContext;
            }
        }

        public ApplicationsUC()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the MenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            //GET THE SELECTED APPLICATION
            SPPIDApp selectedApp = null;
            if (lstApps.SelectedItem != null)
            {
                selectedApp = (SPPIDApp)lstApps.SelectedItem;
            }

            vm.MenuItemClicked(mi, selectedApp);
        }
    }
}
