using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Finance.Domain.Aggregate.VoucherApproval
{
    public class Fn_RealisasiVoucher
    {
        [Key]
        public int ID_Fn_RealisasiVoucher { get; set; }
        public int ID_Ms_UnitUsaha { get; set; }
        public DateTime TanggalRealisasi { get; set; }
        public string ModifyStatus { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string StatusRelease { get; set; }
        public int ID_Ms_Bagian { get; set; }
        public string Komentar { get; set; }
    }
}
