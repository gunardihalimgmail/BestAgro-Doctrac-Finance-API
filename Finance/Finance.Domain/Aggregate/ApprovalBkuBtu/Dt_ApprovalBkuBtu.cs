using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Finance.Domain.Aggregate.ApprovalBkuBtu
{
    public class Dt_ApprovalBkuBtu
    {
        [Key]
        public int ID { get; set; }
        public int Form { get; set; }
        public int FormId { get; set; }

        public DateTime TanggalAppr1 { get; set; }
        public DateTime? TanggalAppr2 { get; set; }
        public DateTime? TanggalAppr3 { get; set; }
        public DateTime? TanggalAppr4 { get; set; }
        public DateTime? TanggalAppr5 { get; set; }
        public DateTime? TanggalAppr6 { get; set; }

        public string RowStatus { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }

        public string StatusAppr1 { get; set; }
        public string StatusAppr2 { get; set; }
        public string StatusAppr3 { get; set; }
        public string StatusAppr4 { get; set; }
        public string StatusAppr5 { get; set; }
        public string StatusAppr6 { get; set; }

        public string Comment1 { get; set; }
        public string Comment2 { get; set; }
        public string Comment3 { get; set; }
        public string Comment4 { get; set; }
        public string Comment5 { get; set; }
        public string Comment6 { get; set; }

        public int? IDLogin1 { get; set; }
        public int? IDLogin2 { get; set; }
        public int? IDLogin3 { get; set; }
        public int? IDLogin4 { get; set; }
        public int? IDLogin5 { get; set; }
        public int? IDLogin6 { get; set; }
    }
}
