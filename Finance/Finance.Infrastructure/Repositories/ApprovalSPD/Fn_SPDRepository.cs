using BestAgroCore.Infrastructure.Data.EFRepositories;
using Finance.Domain.Aggregate.ApprovalBkuBtu;
using Finance.Domain.Aggregate.ApprovalSPD;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Infrastructure.Repositories.ApprovalSPD
{
    public class Fn_SPDRepository : EfRepository<Fn_SPD>, IFn_SPDRepository
    {
        private readonly FinanceContext _context;

        public Fn_SPDRepository(FinanceContext context) : base(context)
        {
            _context = context;
        }
    }
}
