using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Finance.Domain.Aggregate.ApprovalBkuBtu
{
    public class Fn_BTU
    {
        public int ID_Fn_BTU { get; set; }
        public int ID_Ms_UnitUsaha { get; set; }
        public int ID_Ms_Bagian { get; set; }
        public int ID_Ac_Akun { get; set; }
        public int ID_Ms_Keuangan { get; set; }
        public int ID_Ms_CabangBank { get; set; }
        public string Nomor { get; set; }
        public DateTime Tanggal { get; set; }
        public string NomorReferensi { get; set; }
        public string Keterangan { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public string NomorVoucher { get; set; }
        public DateTime TanggalVoucher { get; set; }
        public string NomorBank { get; set; }
        public DateTime TanggalTransfer { get; set; }
        public string ModifyStatus { get; set; }
        public string FlagPerson { get; set; }
        public int ID_Ms_Person { get; set; }
        public int ID_Ms_RekeningPerson { get; set; }
        public int ID_Ms_MataUang { get; set; }
        public string FlagCaraPembayaran { get; set; }
        public int ID_Ms_MataUang_Voucher { get; set; }
        public string NomorGiro { get; set; }
        public DateTime TanggalGiro { get; set; }
        public string DiterimaDari { get; set; }
        public string FlagInternal { get; set; }
        public string Pembebanan { get; set; }
        public DateTime TanggalPerkiraan { get; set; }





























    }
}
