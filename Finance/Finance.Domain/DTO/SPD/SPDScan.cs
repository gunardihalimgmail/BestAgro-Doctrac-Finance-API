using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Domain.DTO.SPD
{
    public class SPDScan
    {
        public int ID_Fn_SPD_Scan { get; set; }
        public int ID_Fn_SPD { get; set; }
        public string Nomor { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public string ModifyStatus { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifyTime { get; set; }
    }

    public class DownloadRequest
    {
        public int id_ms_login { get; set; }
        public int id_fn_bku { get; set; }
    }
}
