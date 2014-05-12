
namespace Fluor.ProjectSwitcher.Message
{
    public class MessageChangePlantName
    {
        public string PlantName { get; set; }

        public MessageChangePlantName(string plantName)
        {
            PlantName = plantName;
        }
    }
}
