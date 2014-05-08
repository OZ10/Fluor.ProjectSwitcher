using System.Windows.Media;
using System.Windows;
using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace Fluor.ProjectSwitcher
{
    public class SPPIDProject : Class.ProjectBase 
    {
        //public string Name { get; set; }
        //public string ID { get; set; }
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
                    this.HighLightColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC27000");
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
                //TODO RaisePropertyChanged("HighLightColor");
            }
        }

        public SPPIDProject()
        {
            IsEnabled = true;
            this.HighLightColor = new SolidColorBrush(Colors.Black);
        }
    }
}
