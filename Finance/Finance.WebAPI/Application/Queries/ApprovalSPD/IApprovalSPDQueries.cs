using Finance.Domain.Aggregate.ApprovalBkuBtu;
using Finance.Domain.Aggregate.ApprovalSPD;
using Finance.Domain.DTO.SPD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.WebAPI.Application.Queries.ApprovalSPD
{
    public interface IApprovalSPDQueries
    {
        Task<List<DokumenSPD>> GetToKirim(int id_ms_login, string bagian, string divisi, string pt);
        Task<KaryawanInfo> GetInfoKaryawan(int id_ms_login);
        Task<List<OpsiTujuan>> GetOpsiTujuanEST();
        Task<List<OpsiTujuan>> GetOpsiTujuanJKT();
        Task<Dt_DocStatus> GetDocStatusKirim(int id, string nomor);
        Task<Dt_DocStatus> GetDocStatus(int id);
        Task<string> TolakDir(string nomor);
        Dt_DocStatus GetDocStatusbyID(int id);
        string GetDivisi(int id_ms_login);
        Task<Dt_DocProcessStatus> GetDocProcessStatus(string nomor, string jenis);
        Task<Dt_DocFlowSetting> getDtDocFlowSetting(string flowtype, string bagian, string jenis, int status);
        List<Ms_MataUang> getMsMataUang();
        Fn_SPD getFnSPD(int id_fn_spd);
        Task<Fn_SPD> getFnSPDTolak(int refid);
        Dt_DocDeliveryStatus getDocDeliveryStatus(string nomor, string jenis);
        Task<Dt_DocFlowType> getFlow(string flowtype, string bagian);
        Task<List<DokumenSPD>> GetToTerima(int id_ms_login);
        Task<List<NoteList>> getDirNotes(string nomor);
        Task<List<SPDScan>> getSpdScan(int id_fn_spd);
        Task<List<Fn_SPD>> getCutOffSpdEstate(int refid);
        Task<List<Fn_SPD>> getCutOffSpdHO(int refid);
        Task<List<Fn_SPD_Count>> getDownloadedScanByDiv(int id_ms_bagian, int id_ms_divisi);
        Task<List<DokumenKirimOutstanding>> GetKirimNotTerima(int id_ms_login);
        Task<List<DokumenSPD>> GetSpdDitolak(int id_ms_login);
    }
}
