using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Finance.Domain.Aggregate.ApprovalSPD
{
    public class Fn_SPD_Count
    {
        [Key]
        public int ID_Fn_SPD_Count { get; set; }
        public int ID_Fn_SPD { get; set; }
        public int ID_Ms_Divisi { get; set; }
        public int ID_Ms_Bagian { get; set; }
        public int ID_Ms_Login { get; set; }
        public DateTime DownloadDate { get; set; }
    }
}
