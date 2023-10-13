using BestAgroCore.Infrastructure.Data.EFRepositories;
using Finance.Domain.Aggregate.ApprovalSPD;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Infrastructure.Repositories.ApprovalSPD
{
    public class Dt_DocFlowSettingRepository : EfRepository<Dt_DocFlowSetting>, IDt_DocFlowSettingRepository
    {
        private readonly FinanceContext _context;

        public Dt_DocFlowSettingRepository(FinanceContext context) : base(context)
        {
            _context = context;
        }
    }
}
