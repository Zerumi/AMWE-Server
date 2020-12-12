// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
namespace AMWE_RealTime_Server.Models
{
    public class ChatState
    {
        public uint ID { get; set; }
        public bool IsAccepted { get; set; }
        public string AdminConnectionID { get; set; }
        public Client User { get; set; }
    }
}