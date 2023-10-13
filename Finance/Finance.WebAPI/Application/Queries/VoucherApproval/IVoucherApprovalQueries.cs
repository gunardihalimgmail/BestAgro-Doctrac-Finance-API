using System.Collections.Generic;
using System.Threading.Tasks;
using Finance.Domain.DTO.VoucherApproval.Request;
using Finance.Domain.DTO.VoucherApproval;
using Finance.Domain.Aggregate.Common;
using Finance.Domain.Aggregate.VoucherApproval;


namespace Finance.WebAPI.Application.Queries.VoucherApproval
{
    public interface IVoucherApprovalQueries
    {
        Task<List<VoucherPT>> GetPTSPV(int id_ms_login);

        Task<List<VoucherPT>> GetAllVoucherPT(int id_ms_login, string tanggal);

        Task<List<VoucherPT>> GetAllVoucherPTSPV(int id_ms_login, string tanggal);

        Task<List<VoucherLembar>> GetAllPTVoucherLembarMGMT(int id_ms_login, string tanggal);

        Task<List<VoucherLembar>> GetAllPTVoucherLembarSPV(int id_ms_login, string tanggal);

        Task<List<VoucherLembar>> GetSinglePTVoucherLembarSPV(int id_ms_login, string tanggal, string kode_unitusaha);

        Task<List<VoucherRealisasiDetail>> GetVoucherBKU(int id_ms_login, string tanggal, string pt);

        Task<string> GetRoleVoucher(int id_ms_login);

        Task<List<VoucherApproved>> GetVoucherToApproveMGR();

        Task<List<VoucherApproved>> GetVoucherToApproveDIR();

        Task<List<VoucherApproved>> GetVoucherToApproveCEO();

        Task<List<VoucherApproved>> GetVoucherToReleasedSPV(int id_ms_login);

        Task<List<VoucherRealisasiDetail>> GetVoucherDetailSPV(int id_ms_login, string tanggal);

        Task<List<VoucherRealisasiDetail>> GetVoucherDetailMGMT(int id_ms_login, string tanggal);

        Task<string> GetIsReadyToReleased(int id_ms_login, string tanggal);

        Task<string> GetIsReadyToApproved(string tanggal);

        Task<Fn_RealisasiVoucher_Approval> GetSingleVouApv(string tanggal);

        Task<List<Fn_RealisasiVoucher>> GetVouRealSPV(int id_ms_login, string tanggal);

        Task<List<Fn_RealisasiVoucher_Detail>> GetRemoveVoucherDetail(int id_ms_login, string tanggal, int id_ms_unitusaha);

        Task<Fn_RealisasiVoucher> GetRemoveVoucher(int id_ms_login, string tanggal, int id_ms_unitusaha);
    }
}
