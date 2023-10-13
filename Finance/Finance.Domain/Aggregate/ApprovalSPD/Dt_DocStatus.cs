using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Finance.Domain.Aggregate.ApprovalSPD
{
    public class Dt_DocStatus
    {
        [Key]
        public int ID { get; set; }
        public string Jenis { get; set; }
        public string Nomor { get; set; }
        public int RefId { get; set; }
        public string Perihal { get; set; }
        public string Bagian { get; set; }
        public string FlowType { get; set; }
        public string PT { get; set; }
        public int Status { get; set; }
        public DateTime StatusTime { get; set; }
        public int StatusBy { get; set; }
        public string LastNotes { get; set; }
        public bool IsRejected { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime RequestTime { get; set; }
        public int RequestBy { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string RowStatus { get; set; }
        public DateTime? LastReceivedTime { get; set; }
        public int? LastReceivedBy { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string DirNotes { get; set; }
    }
}
