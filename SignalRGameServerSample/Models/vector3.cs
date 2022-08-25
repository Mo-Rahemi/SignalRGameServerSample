namespace SignalRGameServerSample.Models
{
    public class vector3
    {
        public static vector3 Zero = new vector3(0, 0, 0);
        public vector3() { }
        public vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float x { get; set; } = 0.1f;
        public float y { get; set; } = 0.12f;
        public float z { get; set; } = 0.13f;
    }
}
