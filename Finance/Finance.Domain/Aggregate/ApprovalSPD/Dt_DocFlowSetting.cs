using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Finance.Domain.Aggregate.ApprovalSPD
{
    public class Dt_DocFlowSetting
    {
        [Key]
        public int ID { get; set; }
        public string DocType { get; set; }
        public string Bagian { get; set; }
        public string FlowType { get; set; }
        public int Status { get; set; }
        public string Pic { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }
}
