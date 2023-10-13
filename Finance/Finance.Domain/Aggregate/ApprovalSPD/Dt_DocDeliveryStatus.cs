using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Finance.Domain.Aggregate.ApprovalSPD
{
    public class Dt_DocDeliveryStatus
    {
        [Key]
        public int ID { get; set; }
        public string Jenis { get; set; }
        public string Nomor { get; set; }
        public string FlowType { get; set; }
        public int StatusLevel { get; set; }
        public DateTime SendTime { get; set; }
        public int SendBy { get; set; }
        public string SendByDivisi { get; set; }
        public DateTime? ReceivedTime { get; set; }
        public int? ReceivedBy { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }
}
