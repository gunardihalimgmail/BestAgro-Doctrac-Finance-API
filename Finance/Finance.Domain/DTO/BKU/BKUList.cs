using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Domain.DTO.BKU
{
    public class BKUList
    {
        public int ID { get; set; }
        public string DocKey { get; set; }
        public string Nomor { get; set; }
        public DateTime Tanggal { get; set; }
        public string TanggalStr { get; set; }
        public string PT { get; set; }
        public decimal Nilai { get; set; }
        public string Supplier { get; set; }
        public string Cara { get; set; }
        public string Keterangan { get; set; }
        public string Status { get; set; }
        public int ID_Ms_Supplier { get; set; }
        public string KetRekening { get; set; }
        public string IsDownload { get; set; }
    }
}
