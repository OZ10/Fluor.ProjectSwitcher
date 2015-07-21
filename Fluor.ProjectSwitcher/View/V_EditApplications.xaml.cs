using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fluor.ProjectSwitcher.View
{
    /// <summary>
    /// Interaction logic for V_EditApplications.xaml
    /// </summary>
    public partial class V_EditApplications : UserControl
    {
        public ViewModel.VM_EditApplications VM
        {
            get
            {
                return (ViewModel.VM_EditApplications)this.DataContext;
            }
        }
        public V_EditApplications()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            VM.SelectedApplication = (Class.TopApplication)e.NewValue;
        }

        private void btnAddNewApplication_Click(object sender, RoutedEventArgs e)
        {
            VM.AddNewApplication();
        }

        private void btnAddnewContextMenu_Click(object sender, RoutedEventArgs e)
        {
            VM.AddNewContextMenu();
        }

        private void ChangeListViewItem_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;

            Class.ContextMenu cm = b.DataContext as Class.ContextMenu;

            if (cm != null)
            {
                switch (b.Tag.ToString())
                {
                    case "Edit":
                        VM.SelectedContextMenu = cm;
                        break;
                    case "Delete":
                        VM.DeleteContextMenu(cm);
                        break;
                    default:
                        break;
                }
            }
        }

        private void btnEditOk_Click(object sender, RoutedEventArgs e)
        {
            VM.OkButton_Clicked();
        }

        private void btnEditCancel_Click(object sender, RoutedEventArgs e)
        {
            VM.CancelButton_Clicked();
        }

        private void btnDeleteApplication_Click(object sender, RoutedEventArgs e)
        {
            VM.DeleteButton_Clicked();
        }
    }
}
