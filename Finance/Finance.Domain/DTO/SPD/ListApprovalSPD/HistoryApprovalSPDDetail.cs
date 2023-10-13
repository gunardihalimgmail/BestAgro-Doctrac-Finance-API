using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Finance.Domain.DTO.SPD.ListApprovalSPD
{
    public class HistoryApprovalSPDDetail
    {
        [Key]
        public int ID { get; set; }
        public int Level { get; set; }
        public string Keterangan { get; set; }
        public string Tanggal { get; set; }
        public DateTime ReceivedTime { get; set; }
        public string Status { get; set; }
    }
}
