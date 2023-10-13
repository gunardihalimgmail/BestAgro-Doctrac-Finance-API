using BestAgroCore.Infrastructure.Data.EFRepositories;
using Finance.Domain.Aggregate.ApprovalSPD;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Infrastructure.Repositories.ApprovalSPD
{
    public class Fn_SPD_CountRepository : EfRepository<Fn_SPD_Count>, IFn_SPD_CountRepository
    {
        private readonly FinanceScanContext _context;

        public Fn_SPD_CountRepository(FinanceScanContext context) : base(context)
        {
            _context = context;
        }
    }
}
