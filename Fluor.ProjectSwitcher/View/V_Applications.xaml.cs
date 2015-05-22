using Fluor.ProjectSwitcher.Class;
using System.Windows;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher.View
{
    /// <summary>
    /// Interaction logic for ApplicationsUC.xaml
    /// </summary>
    public partial class V_Applications : UserControl
    {
        public ViewModel.VM_Applications vm
        {
            get
            {
                return (ViewModel.VM_Applications)DataContext;
            }
        }

        public V_Applications()
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
