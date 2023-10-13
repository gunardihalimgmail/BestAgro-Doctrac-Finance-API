using System;
using System.ComponentModel.DataAnnotations;

namespace Finance.Domain.DTO.VoucherApproval
{
    public class VoucherLembar
    {
        public int id { get; set; }
        public int id_ms_unitusaha { get; set; }
        public string kodept { get; set; }
        public string namapt { get; set; }
        public string rekening { get; set; }
        public string totalnominal { get; set; }
        public string totallembar { get; set; }
    }
}
