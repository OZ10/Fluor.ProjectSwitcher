using Fluor.ProjectSwitcher.Class;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher.View
{
    /// <summary>
    /// Interaction logic for ApplicationsUC.xaml
    /// </summary>
    public partial class ViewAddNew : UserControl
    {
        public ViewModel.ViewModelAddNew vm
        {
            get
            {
                return (ViewModel.ViewModelAddNew)DataContext;
            }
        }

        public ViewAddNew()
        {
            InitializeComponent();
        }

        //private void muApplication_Click(object sender, RoutedEventArgs e)
        //{
        //    MenuItem mi = (MenuItem)sender;

        //    Utilities.OpenFolder(mi.CommandParameter.ToString());
        //}
    }
}
