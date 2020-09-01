namespace AMWE_RealTime_Server.Models
{
    public class ProcessesDetails
    {
        public uint ChangesCount { get; set; }
        public string[] ProcessesNow { get; set; }
        public ProccesChange[] ProccesChanges { get; set; }
    }
}