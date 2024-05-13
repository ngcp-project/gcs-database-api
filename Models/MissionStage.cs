using System.Text.Json;

namespace Database.Models
{
    public class MissionStage
    {
        public string stageName { get; set; }
        public Stage_Enum stageStatus { get; set; }
        public VehicleData[] vehicleKeys { get; set; }

        public MissionStage(string stageName, VehicleData[] vehicleKeys)
        {
            this.stageName = stageName;
            this.stageStatus = Stage_Enum.NOT_STARTED;
            this.vehicleKeys = vehicleKeys;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}