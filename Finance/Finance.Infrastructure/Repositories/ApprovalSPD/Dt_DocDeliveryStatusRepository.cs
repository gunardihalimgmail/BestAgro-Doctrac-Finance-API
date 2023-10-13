using BestAgroCore.Infrastructure.Data.EFRepositories;
using Finance.Domain.Aggregate.ApprovalSPD;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Infrastructure.Repositories.ApprovalSPD
{
    public class Dt_DocDeliveryStatusRepository : EfRepository<Dt_DocDeliveryStatus>, IDt_DocDeliveryStatusRepository
    {
        private readonly FinanceContext _context;

        public Dt_DocDeliveryStatusRepository(FinanceContext context) : base(context)
        {
            _context = context;
        }
    }
}
