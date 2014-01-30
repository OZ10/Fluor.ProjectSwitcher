using System.Windows.Media;
using System.ComponentModel;
using System.Windows;

namespace Fluor.SPPID.ProjectSwitcher
{
    class SPPIDProject : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string IniFile { get; set; }
        public string PIDPath { get; set; }
        public string SPENGPath { get; set; }
        public bool IsEnabled { get; set; }

        private Visibility isVisible;
        public Visibility IsVisible
        {
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

        private string plantName;
        public string PlantName
        {
            get
            {
                return plantName;
            }
            set
            {
                plantName = value;
                if (value == "SEP" | value == "HEADER")
                {
                    IsEnabled = false;
                }
            }
        }

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
                    this.HighLightColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC0504D");
                }
                else
                {
                    this.HighLightColor = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private SolidColorBrush _highLightColor;
        public SolidColorBrush HighLightColor
        {
            get
            {
                return _highLightColor;
            }
            set
            {
                _highLightColor = value;
                NotifyPropertyChanged("HighLightColor");
            }
        }

        public SPPIDProject()
        {
            IsEnabled = true;
            this.HighLightColor = new SolidColorBrush(Colors.Black);
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
