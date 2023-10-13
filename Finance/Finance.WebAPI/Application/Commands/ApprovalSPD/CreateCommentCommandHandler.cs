using BestAgroCore.Common.Domain;
using BestAgroCore.Infrastructure.Data.EFRepositories.Contracts;
using Finance.Domain.Aggregate.ApprovalSPD;
using Finance.Infrastructure;
using Finance.WebAPI.Application.Queries.ApprovalSPD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.WebAPI.Application.Commands.ApprovalSPD
{
    public class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand>
    {
        private readonly IUnitOfWork<FinanceContext> _uow;
        private readonly IDt_DocStatusRepository _Dt_DocStatusRepository;
        private readonly IApprovalSPDQueries _approvalSPDQueries;

        public CreateCommentCommandHandler(IUnitOfWork<FinanceContext> uow,
            IDt_DocStatusRepository Dt_DocStatusRepository,
            IApprovalSPDQueries approvalSPDQueries)
        {
            _uow = uow;
            _Dt_DocStatusRepository = Dt_DocStatusRepository;
            _approvalSPDQueries = approvalSPDQueries;
        }

        public async Task Handle(CreateCommentCommand command, CancellationToken cancellationToken)
        {
            try
            {

                var qDoc = await _approvalSPDQueries.GetDocStatusKirim(command.ID, command.Nomor);

                qDoc.LastNotes = command.CommentNotes ?? "";


                _Dt_DocStatusRepository.Update(qDoc);
                await _uow.CommitAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
