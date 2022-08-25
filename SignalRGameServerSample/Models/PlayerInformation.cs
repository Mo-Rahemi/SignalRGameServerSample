using System.Text.Json.Serialization;

namespace SignalRGameServerSample.Models
{
    public class PlayerInformation
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        [JsonIgnore]
        public string GameId { get; set; } = "";
        public vector3 Position { get; set; } = vector3.Zero;
        public float LookAt { get; set; } = 0f;
    }
}
