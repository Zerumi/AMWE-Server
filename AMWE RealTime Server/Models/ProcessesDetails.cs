// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
namespace AMWE_RealTime_Server.Models
{
    public class ProcessesDetails
    {
        public uint ChangesCount { get; set; }
        public string[] ProcessesNow { get; set; }
        public ProcessChange[] ProccesChanges { get; set; }
    }
}