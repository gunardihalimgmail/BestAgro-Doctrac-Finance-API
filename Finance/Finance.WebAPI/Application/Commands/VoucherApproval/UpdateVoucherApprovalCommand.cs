using BestAgroCore.Common.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.WebAPI.Application.Commands.VoucherApproval
{
    public class UpdateVoucherApprovalCommand : ICommand
    {
        public int lastmodifiedby { get; set; }
        public DateTime tanggalrealisasi { get; set; }
    }
}
