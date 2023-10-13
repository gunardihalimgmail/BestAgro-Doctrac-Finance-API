using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Domain.DTO.BKU
{

    public class BKUDetailAndOPLPB
    {
        public List<BKUDetail> bkuDetail { get; set; }
        public List<BKUOPLPBDetail> bkuOpLpbDetail { get; set; }
        public BKUApprovalDetailHistory bkuDetailHistory { get; set; }
    }


    public class BKUDetail
    {
        public string Id { get; set; }
        public int IDTransAkun { get; set; }
        public int IDTransSPD { get; set; }
        public string Jenis { get; set; }
        public string Sumber { get; set; }
        public string Nomor { get; set; }
        public string KodeAkun { get; set; }
        public string NamaAkun { get; set; }
        public string Detail { get; set; }
        public decimal Harga { get; set; }
        public decimal Total { get; set; }
    }

    public class BKUOPLPBDetail
    {
        public string ID { get; set; }
        public string NomorOP { get; set; }
        public string TanggalOP { get; set; }
        public string NomorLPB { get; set; }
        public string TanggalLPB { get; set; }
        public string Tagihan { get; set; }
        public string Type { get; set; }
    }

    public class BKUApprovalDetailHistory
    {
        public string ID { get; set; }
        public string ApprovalSPVFinance { get; set; }
        public string ApprovalManagerFinance { get; set; }
        public string ApprovalManagerBNC { get; set; }
        public string ApprovalDir1 { get; set; }
        public string ApprovalDir2 { get; set; }
    }
}
