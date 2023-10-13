using BestAgroCore.Infrastructure.Data.EFRepositories;
using Finance.Domain.Aggregate.ApprovalSPD;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Infrastructure.Repositories.ApprovalSPD
{
    public class Dt_DocProcessStatusRepository : EfRepository<Dt_DocProcessStatus>, IDt_DocProcessStatusRepository
    {
        private readonly FinanceContext _context;

        public Dt_DocProcessStatusRepository(FinanceContext context) : base(context)
        {
            _context = context;
        }
    }
}
