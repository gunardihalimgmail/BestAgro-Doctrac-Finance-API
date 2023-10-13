using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Finance.Domain.Aggregate.VoucherApproval
{
    public class Ms_Keuangan_RptRealVoucher
    {
        [Key]
        public int ID_Ms_Keuangan_RptRealVoucher { get; set; }
        public int? ID_Ms_Keuangan { get; set; }
        public int? Id_Ms_Login_Spv { get; set; }
        public string ModifyStatus { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }
}
