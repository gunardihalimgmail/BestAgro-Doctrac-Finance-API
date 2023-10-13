using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Finance.Domain.Aggregate.ApprovalBkuBtu
{
    public class Fn_BKU_SPD
    {
        [Key]
        public int ID_Fn_BKU_SPD { get; set; }
        public int ID_Fn_BKU { get; set; }
        public int ID_Fn_SPD { get; set; }
        public int ID_Ms_MataUang { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Jumlah { get; set; }

        public string Keterangan { get; set; }
        public string ModifyStatus { get; set; }
    }
}
