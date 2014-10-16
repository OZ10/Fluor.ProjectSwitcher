using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using Fluor.ProjectSwitcher.Base.Class;

namespace Fluor.ProjectSwitcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private void tile_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Grid grid = (Grid)sender;
            //Tile tile = (Tile)sender;
            Messenger.Default.Send<GenericMessage<Grid>>(new GenericMessage<Grid>(grid));
        }

        private void muProject_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            Fluor.ProjectSwitcher.Base.Class.Utilities.OpenFolder(mi.CommandParameter.ToString());
        }

        private void tbApplicationName_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            Messenger.Default.Send<GenericMessage<TextBlock>>(new GenericMessage<TextBlock>(tb));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            Messenger.Default.Send<GenericMessage<MenuItem>>(new GenericMessage<MenuItem>(mi));
        }
    }
}
