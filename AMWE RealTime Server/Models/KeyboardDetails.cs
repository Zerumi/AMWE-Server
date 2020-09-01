namespace AMWE_RealTime_Server.Models
{
    public class KeyboardDetails
    {
        public uint KeyPressedCount { get; set; }
        public KeyPressedInfo[] PressedInfo { get; set; }
    }
}