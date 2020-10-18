using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMWE_RealTime_Server.Models
{
    public class VersionFile
    {
        public int ID { get; set; }
        public int VersionID { get; set; }
        public string filename { get; set; }
        public byte[] filebytes { get; set; }

        [JsonIgnore]
        public virtual Version Version { get; set; }
    }
}