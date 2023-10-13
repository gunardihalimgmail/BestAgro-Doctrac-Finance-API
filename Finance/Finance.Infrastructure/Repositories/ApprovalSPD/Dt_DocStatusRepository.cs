using BestAgroCore.Infrastructure.Data.EFRepositories;
using Finance.Domain.Aggregate.ApprovalSPD;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Infrastructure.Repositories.ApprovalSPD
{
    public class Dt_DocStatusRepository : EfRepository<Dt_DocStatus>, IDt_DocStatusRepository
    {
        private readonly FinanceContext _context;

        public Dt_DocStatusRepository(FinanceContext context) : base(context)
        {
            _context = context;
        }
    }
}
