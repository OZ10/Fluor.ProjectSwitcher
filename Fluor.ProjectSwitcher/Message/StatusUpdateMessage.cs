using System.Windows;

namespace Fluor.ProjectSwitcher.Message
{
    public class StatusUpdateMessage
    {
        public Visibility Visibility { get; set; }
        public string StatusText { get; set; }

        public StatusUpdateMessage(Visibility visibility, string statusText = "")
        {
            Visibility = visibility;
            StatusText = statusText;
        }
    }
}
