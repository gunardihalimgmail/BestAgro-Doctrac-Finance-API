using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Domain.DTO.SPD
{
    public class DokumenFlowStatus
    {
        public int ID { get; set; }
        public int Level { get; set; }
        public string Keterangan { get; set; }
        public string Tanggal { get; set; }
        public string Status { get; set; }
        public string LevelStyle { get { return string.Format("level{0}", Level); } }
    }
}
