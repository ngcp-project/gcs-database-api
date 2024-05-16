using System.Text.Json;


namespace Database.Models
{
    public class MissionInfo
    {

        public string missionName { get; set; }
        public string currentStageId { get; set; }
        public MissionStage[] stages { get; set; }
        
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}