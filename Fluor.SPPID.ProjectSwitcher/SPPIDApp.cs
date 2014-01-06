using System.ComponentModel;

namespace Fluor.SPPID.ProjectSwitcher
{
    class SPPIDApp : INotifyPropertyChanged
    {
        public string AppName { get; set; }
        public string ParentApp { get; set; }
        public string Exe { get; set; }

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
