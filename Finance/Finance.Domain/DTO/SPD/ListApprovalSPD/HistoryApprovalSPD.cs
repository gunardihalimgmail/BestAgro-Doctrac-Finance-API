using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Finance.Domain.DTO.SPD.ListApprovalSPD
{
    public class HistoryApprovalSPD
    {
        [Key]
        public int ID { get; set; }
        public string Nomor { get; set; }
        public string Perihal { get; set; }
        public string FlowType { get; set; }
        public string RequestByName { get; set; }
        public string RequestTimeStr { get; set; }
        public string PIC { get; set; }
        public string Jenis { get; set; }
        public string StatusTimeStr { get; set; }
        public string LastReceivedByName { get; set; }
        public string LastReceivedTimeStr { get; set; }
        public string LastNotes { get; set; }
        public int IsRejected { get; set; }
    }
}
