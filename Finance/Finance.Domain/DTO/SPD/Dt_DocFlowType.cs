using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Domain.DTO.SPD
{
    public class Dt_DocFlowType
    {
        public int ID { get; set; }
        public string FlowType { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string RowStatus { get; set; }
    }
}
