using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Domain.DTO.VoucherApproval
{
    public class VoucherRealisasiDetail
    {
        public string id { get; set; }
        public string unitusaha { get; set; }
        public string keuangan { get; set; }
        public DateTime tanggal { get; set; }
        public string bku { get; set; }
        public string giro { get; set; }
        public string voucher { get; set; }
        public Decimal nominal { get; set; }
        public string supplier { get; set; }
        public string keterangan { get; set; }
    }
}
