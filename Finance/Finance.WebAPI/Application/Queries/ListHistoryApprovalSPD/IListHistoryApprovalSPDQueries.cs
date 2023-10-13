using Finance.Domain.DTO.SPD;
using Finance.Domain.DTO.SPD.ListApprovalSPD;
using Finance.Domain.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.WebAPI.Application.Queries.ListHistoryApprovalSPD
{
    public interface IListHistoryApprovalSPDQueries
    {
        Task<UserDetail> GetUserInfo(int idLogin);
        Task<List<HistoryApprovalSPD>> GetListHistorySPD(int id_ms_login, string pt, string keyword, DateTime startdate, DateTime enddate, bool isuserkebun);
        Task<List<HistoryApprovalSPDDetail>> GetListHistorySPDDetail(string nomor);
        Task<string> GetNomorSPDByID(int id);
        Task<List<DokumenFlowStatus>> getdokflowstatus(string jenis, string nomor);
    }
}
