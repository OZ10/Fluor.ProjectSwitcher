using Fluor.ProjectSwitcher.Class;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher.View
{
    /// <summary>
    /// Interaction logic for ApplicationsUC.xaml
    /// </summary>
    public partial class V_AddNew : UserControl
    {
        public ViewModel.VM_AddNew vm
        {
            get
            {
                return (ViewModel.VM_AddNew)DataContext;
            }
        }

        public V_AddNew()
        {
            InitializeComponent();
        }

        private void btnEditOk_Click(object sender, RoutedEventArgs e)
        {
            //Messenger.Default.Send<Message.M_SimpleAction>(new Message.M_SimpleAction(Message.M_SimpleAction.Action.DisplayTilesTab));
            //Messenger.Default.Send<Message.M_SimpleAction>(new Message.M_SimpleAction(Message.M_SimpleAction.Action.RefreshViews));
            //Messenger.Default.Send<Message.MessageSaveChangesToTile>(new Message.MessageSaveChangesToTile(null, this));
            vm.OkButton_Clicked();
        }

        private void btnAddnewContextMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddAssociatedApplication_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void muApplication_Click(object sender, RoutedEventArgs e)
        //{
        //    MenuItem mi = (MenuItem)sender;

        //    Utilities.OpenFolder(mi.CommandParameter.ToString());
        //}
    }
}
