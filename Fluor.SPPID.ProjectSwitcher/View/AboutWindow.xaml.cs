using System.Windows;
using System.Windows.Controls;

namespace Fluor.SPPID.ProjectSwitcher
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : UserControl
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void gdAbout_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
