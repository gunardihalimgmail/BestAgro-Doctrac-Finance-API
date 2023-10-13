using System;
using System.ComponentModel.DataAnnotations;

namespace Finance.Domain.DTO.VoucherApproval
{
    public class VoucherPT
    {
        public int id { get; set; }
        public string kodept { get; set; }
        public string namapt { get; set; }
        public string komentar { get; set; }
    }
}
