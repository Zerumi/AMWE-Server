// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;

namespace ReportHandler
{
    public class ClientState
    {
        public Client Client { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastLoginDateTime { get; set; }
        public DateTime LastLogoutDateTime { get; set; }

        public bool IgnoreWarning { get; set; }
    }
}
