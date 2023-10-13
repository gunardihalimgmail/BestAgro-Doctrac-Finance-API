using System;
using System.Collections.Generic;
using System.Text;
using BestAgroCore.Infrastructure.Data.EFRepositories;
using Finance.Domain.Aggregate.VoucherApproval;

namespace Finance.Infrastructure.Repositories.VoucherApproval
{
    public class Fn_RealisasiVoucher_ApprovalRepository : EfRepository<Fn_RealisasiVoucher_Approval>, IFn_RealisasiVoucher_ApprovalRepository
    {
        private readonly FinanceContext _context;

        public Fn_RealisasiVoucher_ApprovalRepository(FinanceContext context) : base(context)
        {
            _context = context;
        }
    }
}
