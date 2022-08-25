namespace SignalRGameServerSample.Models
{
    public class Notification
    {
        public string Id { get; set; } = "";
        public string Time { get; set; } = DateTime.Now.ToShortTimeString();
        public string Name { get; set; } = "";
        public string Message { get; set; } = "";
    }
}
