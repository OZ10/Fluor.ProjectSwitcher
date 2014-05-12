using System.Windows;

namespace Fluor.ProjectSwitcher.Message
{
    public class MessageStatusUpdate
    {
        public Visibility Visibility { get; set; }
        public string StatusText { get; set; }

        public MessageStatusUpdate(Visibility visibility, string statusText = "")
        {
            Visibility = visibility;
            StatusText = statusText;
        }
    }
}
