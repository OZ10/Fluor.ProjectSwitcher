
namespace Fluor.SPPID.ProjectSwitcher.Message
{
    public class ChangePlantNameMessage
    {
        public string PlantName { get; set; }

        public ChangePlantNameMessage(string plantName)
        {
            PlantName = plantName;
        }
    }
}
