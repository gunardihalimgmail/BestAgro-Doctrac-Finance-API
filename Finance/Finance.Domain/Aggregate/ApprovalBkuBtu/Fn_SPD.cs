using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Finance.Domain.Aggregate.ApprovalBkuBtu
{
    public class Fn_SPD
    {
        [Key]
        public int ID_Fn_SPD { get; set; }
        public int ID_Ms_UnitUsaha { get; set; }
        public int ID_Ms_Bagian { get; set; }
        public int ID_Ms_Login { get; set; }
        public int ID_Ms_Supplier { get; set; }
        public int ID_Ms_RekeningSupplier { get; set; }
        public int ID_Ms_Divisi { get; set; }
        public int ID_Ms_MataUang { get; set; }
        public int ID_Ac_Akun { get; set; }
        public string Nomor { get; set; }
        public string Tanggal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Nilai { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal NilaiRealisasi { get; set; }


        public string FlagCaraPembayaran { get; set; }
        public string Keterangan { get; set; }
        public string Status { get; set; }
        public string KeteranganStatus { get; set; }
        public string FlagJenisSPD { get; set; }
        public string ModifyStatus { get; set; }
        public string FlagInternal { get; set; }
        public string Perihal { get; set; }
        public int OpsiForm { get; set; }
        public int ID_Ms_SPD_Kategori { get; set; }
    }
}
