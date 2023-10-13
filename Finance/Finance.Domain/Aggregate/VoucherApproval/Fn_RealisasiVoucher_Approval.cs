using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Finance.Domain.Aggregate.VoucherApproval
{
    public class Fn_RealisasiVoucher_Approval
    {
        [Key]
        public int ID_Fn_RealisasiVoucher_Approval { get; set; }
        public DateTime? TanggalRealisasi { get; set; }
        public string? Keterangan { get; set; }
        public int? Status { get; set; }
        public int? Id_Appr1 { get; set; }
        public int? Id_Appr2 { get; set; }
        public int? Id_Appr3 { get; set; }
        public DateTime? TanggalAppr1 { get; set; }
        public DateTime? TanggalAppr2 { get; set; }
        public DateTime? TanggalAppr3 { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
}
