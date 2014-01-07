using System.Windows.Media;
using System.ComponentModel;

namespace Fluor.SPPID.ProjectSwitcher
{
    class SPPIDProject : INotifyPropertyChanged
    {
        public string ProjectName { get; set; }
        public string PlantName { get; set; }
        public string IniFile { get; set; }
        public string PIDPath { get; set; }
        public string SPENGPath { get; set; }

        private bool _isActiveProject;
        public bool IsActiveProject
        {
            get
            {
                return _isActiveProject;
            }
            set
            {
                _isActiveProject = value;

                if (_isActiveProject == true)
                {
                    this.BackgroundColor = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    this.BackgroundColor = null;
                }
            }
        }

        private SolidColorBrush _backgroundColor;
        public SolidColorBrush BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;
                NotifyPropertyChanged("BackgroundColor");
            }
        }

        public SPPIDProject()
        {
            //this.BackgroundColor = new SolidColorBrush(Colors.Red);
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
