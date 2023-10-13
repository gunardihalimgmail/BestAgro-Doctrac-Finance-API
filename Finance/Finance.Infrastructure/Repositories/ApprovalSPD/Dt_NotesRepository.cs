using BestAgroCore.Infrastructure.Data.EFRepositories;
using Finance.Domain.Aggregate.ApprovalSPD;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Infrastructure.Repositories.ApprovalSPD
{
    public class Dt_NotesRepository : EfRepository<Dt_Notes>, IDt_NotesRepository
    {
        private readonly FinanceContext _context;

        public Dt_NotesRepository(FinanceContext context) : base(context)
        {
            _context = context;
        }
    }
}
