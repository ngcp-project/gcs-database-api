namespace Database.Models
{
    public class MissionStagePOST
    {
        public string missionName { get; set; }
        public MissionStage[] stages { get; set; }
    }
}