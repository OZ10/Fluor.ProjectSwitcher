using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Fluor.ProjectSwitcher.Class;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class ViewModelAddNew : ViewModelBase 
    {
        private SwitcherItem selectedItem;
        public SwitcherItem SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }

        private ObservableCollection<ListBox> selectedItemCollection;
        public ObservableCollection<ListBox> SelectedItemCollection
        {
            get
            {
                return selectedItemCollection;
            }
            set
            {
                selectedItemCollection = value;
                RaisePropertyChanged("SelectedItemCollection");
            }
        }

        public ViewModelAddNew()
        {
            //Messenger.Default.Register<GenericMessage<SwitcherItem>>(this, DisplayTileDetails);
            Messenger.Default.Register<Message.MessageUpdateSelectedTile>(this, DisplayTileDetails);
        }

        private void DisplayTileDetails(Message.MessageUpdateSelectedTile msg)
        {
            if (msg.Sender is MainViewModel)
            {
                SelectedItem = msg.SelectedTile;

                SelectedItemCollection = new ObservableCollection<ListBox>();

                ListBox lb = new ListBox();
                lb.Template = (ControlTemplate)System.Windows.Application.Current.Resources["NewTileTemplate"];
                lb.DataContext = SelectedItem;

                SelectedItemCollection.Add(lb);
            }
        }
    }
}
