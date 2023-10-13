using BestAgroCore.Infrastructure.Data.EFRepositories;
using Finance.Domain.Aggregate.ApprovalBkuBtu;

namespace Finance.Infrastructure.Repositories.ApprovalBKU
{
    public class Fn_BKU_CountRepository : EfRepository<Fn_BKU_Count>, IFn_BKU_CountRepository
    {
        private readonly FinanceScanContext _context;

        public Fn_BKU_CountRepository(FinanceScanContext context) : base(context)
        {
            _context = context;
        }
    }
}
