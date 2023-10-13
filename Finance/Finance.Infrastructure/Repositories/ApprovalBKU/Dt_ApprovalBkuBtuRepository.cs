using BestAgroCore.Infrastructure.Data.EFRepositories;
using Finance.Domain.Aggregate.ApprovalBkuBtu;

namespace Finance.Infrastructure.Repositories.ApprovalBKU
{
    public class Dt_ApprovalBkuBtuRepository : EfRepository<Dt_ApprovalBkuBtu>, IDt_ApprovalBkuBtuRepository
    {
        private readonly FinanceContext _context;

        public Dt_ApprovalBkuBtuRepository(FinanceContext context) : base(context)
        {
            _context = context;
        }
    }
}
