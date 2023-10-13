using System;
using System.Collections.Generic;
using System.Text;
using BestAgroCore.Infrastructure.Data.EFRepositories;
using Finance.Domain.Aggregate.VoucherApproval;

namespace Finance.Infrastructure.Repositories.VoucherApproval
{
    public class Ms_Keuangan_RptRealVoucherRepository : EfRepository<Ms_Keuangan_RptRealVoucher>, IMs_Keuangan_RptRealVoucherRepository
    {
        private readonly FinanceContext _context;

        public Ms_Keuangan_RptRealVoucherRepository(FinanceContext context) : base(context)
        {
            _context = context;
        }
    }
}
