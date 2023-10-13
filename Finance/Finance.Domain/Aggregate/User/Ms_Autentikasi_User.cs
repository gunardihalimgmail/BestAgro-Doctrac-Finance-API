using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Finance.Domain.Aggregate.User
{
    public class Ms_Autentikasi_User
    {
        [Key]
        public int ID_Ms_Autentikasi_User { get; set; }
        public int ID_Ms_Autentikasi { get; set; }
        public int ID_Ms_Login { get; set; }
        public DateTime TanggalAwalEfektif { get; set; }
        public DateTime TanggalAkhirEfektif { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifyStatus { get; set; }
    }
}
