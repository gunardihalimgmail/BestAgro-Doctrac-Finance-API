using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Domain.DTO
{
    public class GoogleAuth
    {
        public string AccountSecretKey { get; set; }
    }

    public class DivisiAndJabatan
    {
        public int ID_Ms_Karyawan { get; set; }
        public int ID_Ms_Divisi { get; set; }
        public int ID_Ms_Bagian { get; set; }
        public int ID_Ms_Jabatan { get; set; }
        public string Jabatan { get; set; }
        public string Divisi { get; set; }
    }
}
