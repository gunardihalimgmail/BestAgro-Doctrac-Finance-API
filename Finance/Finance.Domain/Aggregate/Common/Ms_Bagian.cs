using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Finance.Domain.Aggregate.Common
{
    public class Ms_Bagian
    {
        [Key]
        public int ID_Ms_Bagian { get; set; }
        public string Nama { get; set; }
        public string Keterangan { get; set; }
        public string FlagPayroll { get; set; }
        public string FlagERP { get; set; }
        public string Flag { get; set; }
        public string ModifyStatus { get; set; }
    }
}
