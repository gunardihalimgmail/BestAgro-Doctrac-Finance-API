using System.ComponentModel.DataAnnotations;

namespace Finance.Domain.DTO.VoucherApproval
{
    public class VoucherRekening
    {
        public int id_ms_keuangan { get; set; }
        public int id_ms_unitusaha { get; set; }
        public string kodept { get; set; }
        public string namapt { get; set; }
        public string kodekeuangan { get; set; }
        public string nomorrekening { get; set; }
    }
}
