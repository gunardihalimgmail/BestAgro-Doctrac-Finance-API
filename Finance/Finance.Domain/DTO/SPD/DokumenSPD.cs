using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Finance.Domain.DTO.SPD
{
    public class DokumenSPD
    {
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
        public DateTime? LastReceivedTime { get; set; }
        public int? LastReceivedBy { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string RequestByName { get; set; }
        public string RequestTimeStr { get; set; }
        public string LastReceivedByName { get; set; }
        public string LastReceivedTimeStr { get; set; }
        public string StatusByName { get; set; }
        public string StatusTimeStr { get; set; }
        public string Pic { get; set; }
        public string MataUang { get; set; }
        public decimal NilaiPermintaan { get; set; }
        public string NewNotes { get; set; }
        public string DirNotes { get; set; }
        public string RowStatus { get; set; }
        public string DokKey { get { return string.Format("{0}0{1}", this.Jenis, this.RefId.ToString()); } }
        public string IsDownload { get; set; }
    }

    public class DokumenKirimOutstanding
    {
        public int ID { get; set; }
        public string Jenis { get; set; }
        public string Nomor { get; set; }
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
        public DateTime? LastReceivedTime { get; set; }
        public int? LastReceivedBy { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public int RefId { get; set; }
        public string RowStatus { get; set; }
        public string DirNotes { get; set; }
        public string RequestByName { get; set; }
        public string RequestTimeStr { get; set; }
        public string LastReceivedByName { get; set; }
        public string LastReceivedTimeStr { get; set; }
        public string StatusByName { get; set; }
        public string StatusTimeStr { get; set; }
        public string Pic { get; set; }

        [NotMapped]
        public string DokKey { get { return string.Format("{0}0{1}", this.Jenis, this.RefId.ToString()); } }
        
    }


    public class KaryawanInfo
    {
        public int ID_Ms_Karyawan { get; set; }
        public int ID_Ms_Divisi { get; set; }
        public int ID_Ms_Bagian { get; set; }
        public int ID_Ms_Jabatan { get; set; }
        public string PT { get; set; }
        public string Bagian { get; set; }
        public string Divisi { get; set; }
        public string Role { get; set; }
    }

    public class ResultDataListKirimTerima
    {

        public KaryawanInfo DataKaryawan { get; set; }
        public List<DokumenSPD> SpdKirimTerimaList { get; set; }
    }
}
