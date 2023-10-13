using Finance.Domain.Aggregate.ApprovalBkuBtu;
using Finance.Domain.Aggregate.User;
using Finance.Domain.DTO;
using Finance.Domain.DTO.BKU;
using Finance.Domain.DTO.SPD;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Finance.WebAPI.Application.Queries.ApprovalBKU
{
    public interface IApprovalBKUQueries
    {
        Task<List<BKUSPD>> getBKUSPD(int id_fn_bku);
        Task<List<SPDScan>> getSPDScan(List<int> id_fn_spd);
        Task<List<BKUList>> GetListApprovalBKU(int id_ms_login);
        Task<List<BKUDetail>> GetDetailApprovalBKU(string replacedNomor);
        Task<List<BKUOPLPBDetail>> GetDetailApprovalBKUOPLPB(string replacedNomor);
        Task<DivisiAndJabatan> getDivisiAndJabatan(int id_ms_login);
        Task<GoogleAuth> getAccountSecretKey(int id_ms_login);
        Task<Fn_BKU> getIdBku(string nomor);
        Task<List<Fn_BKU_Count>> getFnBkuCount(List<int> id_fn_bku, int id_ms_divisi, int id_ms_bagian, int id_ms_jabatan);
        Task<Fn_BKU> getNomor(int id_fn_bku);
        Task<Ms_Autentikasi> getMsAutentikasi(int id_ms_login);
        Task<Fn_BKU> getIDBKU(string nomor);
        Task<Fn_BTU> getIDBTU(string nomor);
        Task<Dt_ApprovalBkuBtu> approvalBKU(int id_fn_bku);
        Task<Dt_ApprovalBkuBtu> approvalBTU(int id_fn_btu);
        Task<BKUApprovalDetailHistory> GetDetailApprovalBKUHistory(string replacedNomor);

    }
}
