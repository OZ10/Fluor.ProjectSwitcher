using Fluor.ProjectSwitcher.Class;
using GalaSoft.MvvmLight.Messaging;
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

        //private void muApplication_Click(object sender, RoutedEventArgs e)
        //{
        //    MenuItem mi = (MenuItem)sender;

        //    Utilities.OpenFolder(mi.CommandParameter.ToString());
        //}

        private void tbApplicationName_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            vm.DisplayContextMenus(tb);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            Messenger.Default.Send<GenericMessage<MenuItem>>(new GenericMessage<MenuItem>(mi));
        }

        private void muApplication_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            //TODO The binding is not working correctly here; I'm not sure why. For some reason the
            //     The ContextMenu class is bound to the header property of the MenuItem.
            Class.ContextMenu contextMenu = (Class.ContextMenu)mi.Header;

            Utilities.OpenFolder(contextMenu.Value);
            //Fluor.ProjectSwitcher.Class.Utilities.OpenFolder(mi.CommandParameter.ToString());
        }
    }
}
