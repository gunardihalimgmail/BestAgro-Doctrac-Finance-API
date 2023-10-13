using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.Domain.DTO.VoucherApproval.Request
{
    public class VoucherApprovalRequest
    {
        public int id_fn_realisasivoucher { get; set; }
        public int id_ms_login { get; set; }
        public int id_ms_unitusaha { get; set; }
        public DateTime tanggal { get; set; }
        public string pt { get; set; }
    }

    public class CountRequest
    {
        public int id_ms_login { get; set; }
        public string storedProcedure { get; set; }
    }
}
