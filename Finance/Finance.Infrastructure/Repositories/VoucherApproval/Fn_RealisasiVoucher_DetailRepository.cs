using System;
using System.Collections.Generic;
using System.Text;
using BestAgroCore.Infrastructure.Data.EFRepositories;
using Finance.Domain.Aggregate.VoucherApproval;

namespace Finance.Infrastructure.Repositories.VoucherApproval
{
    public class Fn_RealisasiVoucher_DetailRepository : EfRepository<Fn_RealisasiVoucher_Detail>, IFn_RealisasiVoucher_DetailRepository
    {
        private readonly FinanceContext _context;

        public Fn_RealisasiVoucher_DetailRepository(FinanceContext context) : base(context)
        {
            _context = context;
        }
    }
}