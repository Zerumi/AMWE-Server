using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHandler.Version
{
    public class VersionFile
    {
        public int ID { get; set; }
        public string filename { get; set; }
        public byte[] filebytes { get; set; }
    }
}
