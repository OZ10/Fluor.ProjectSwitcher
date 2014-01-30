using System.ComponentModel;
using System.Windows;

namespace Fluor.SPPID.ProjectSwitcher
{
    class SPPIDApp : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Exe { get; set; }
        public string ExeFullPath { get; set; }
        public bool IsEnabled { get; set; }

        private Visibility isVisible;
        public Visibility IsVisible {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;

                //IF THE APP IS HIDDEN SET IT TO BE DISABLED
                if (value == Visibility.Collapsed)
                {
                    IsEnabled = false;
                }
            }
        }

        private string parentApp;
        public string ParentApp {
            get
            {
                return parentApp;
            }
            set
            {
                parentApp = value;
                if (value == "SEP" | value == "HEADER")
                {
                    IsEnabled = false;
                }
            }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }

        public SPPIDApp()
        {
            IsEnabled = true;
        }

        private void NotifyPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
