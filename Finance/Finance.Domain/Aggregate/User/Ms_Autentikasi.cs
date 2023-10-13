using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Finance.Domain.Aggregate.User
{
    public class Ms_Autentikasi
    {
        [Key]
        public int ID_Ms_Autentikasi { get; set; }
        public string Account { get; set; }
        public string AccountSecretKey { get; set; }
        public string ManualEntryKey { get; set; }
        public string QrCodeSetupImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifyStatus { get; set; }
    }
}
