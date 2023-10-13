using BestAgroCore.Common.Infrastructure.Data.Contracts;
using BestAgroCore.Infrastructure.Data.DapperRepositories;
using Dapper;
using Finance.Domain.Aggregate.ApprovalBkuBtu;
using Finance.Domain.Aggregate.ApprovalSPD;
using Finance.Domain.DTO.SPD;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.WebAPI.Application.Queries.ApprovalSPD
{
    public class ApprovalSPDQueries : IApprovalSPDQueries
    {
        private readonly IDbSQLConectionFactory _dbConnectionFactorySQL;

        public ApprovalSPDQueries(IDbSQLConectionFactory dbConnectionFactorySQL)
        {
            _dbConnectionFactorySQL = dbConnectionFactorySQL;
        }


        public async Task<List<DokumenSPD>> GetToKirim(int id_ms_login, string bagian, string divisi, string pt)
        {
            try
            {

                var listDoc = new List<DokumenSPD>();

                if (id_ms_login == 200 || id_ms_login == 201 || id_ms_login == 202)
                {
                    // User bisa buka All PT
                    List<string> ListKodeUnitUsaha = new List<string>();

                    ListKodeUnitUsaha.Add("W1");
                    ListKodeUnitUsaha.Add("W2");
                    ListKodeUnitUsaha.Add("B1");
                    ListKodeUnitUsaha.Add("B2");
                    ListKodeUnitUsaha.Add("B3");
                    ListKodeUnitUsaha.Add("H1");
                    ListKodeUnitUsaha.Add("H2");
                    ListKodeUnitUsaha.Add("T1");
                    ListKodeUnitUsaha.Add("T2");
                    ListKodeUnitUsaha.Add("T3");
                    ListKodeUnitUsaha.Add("SC1");
                    ListKodeUnitUsaha.Add("SC2");
                    ListKodeUnitUsaha.Add("KL");
                    ListKodeUnitUsaha.Add("KL2");
                    ListKodeUnitUsaha.Add("BA");
                    ListKodeUnitUsaha.Add("BE");

                    foreach (var item in ListKodeUnitUsaha.ToList())
                    {
                        var qry = "usp_Dt_GetDokumenBelumRequest";

                        DynamicParameters parameters = new DynamicParameters();

                        parameters.Add("@KodeUnitUsaha", item, DbType.String, ParameterDirection.Input);

                        var data = await new DapperRepository<DokumenSPD>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                            .QueryStoredProcedureAsync(qry, parameters);


                        foreach (var doc in data.ToList())
                        {
                            listDoc.Add(doc);
                        }
                    }

                    return listDoc;
                }
                else
                {

                    if (bagian == "EST" || bagian == "FAC")
                    {
                        if (divisi == "FINE")
                        {

                            var qry = "usp_Dt_GetDokumenBelumRequest";

                            DynamicParameters parameters = new DynamicParameters();

                            parameters.Add("@KodeUnitUsaha", pt, DbType.String, ParameterDirection.Input);

                            var data = await new DapperRepository<DokumenSPD>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                                .QueryStoredProcedureAsync(qry, parameters);

                            
                            return data.ToList();
                        }
                        else
                        {
                            return new List<DokumenSPD>();
                        }
                    }
                    else
                    {
                        var qry = "usp_Dt_GetDokumenToKirim";

                        DynamicParameters parameters = new DynamicParameters();

                        parameters.Add("@IdLogin", id_ms_login, DbType.String, ParameterDirection.Input);

                        var data = await new DapperRepository<DokumenSPD>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                            .QueryStoredProcedureAsync(qry, parameters);

                        return data.ToList();
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<DokumenSPD>> GetToTerima(int id_ms_login)
        {
            try
            {
                var qry = "usp_Dt_GetDokumenToTerima";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@IdLogin", id_ms_login, DbType.String, ParameterDirection.Input);

                var data = await new DapperRepository<DokumenSPD>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryStoredProcedureAsync(qry, parameters);

                return data.ToList();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<DokumenKirimOutstanding>> GetKirimNotTerima(int id_ms_login)
        {
            try
            {
                var qry = "usp_Dt_GetDokumenKirimNotTerima";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@IdLogin", id_ms_login, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<DokumenKirimOutstanding>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryStoredProcedureAsync(qry, parameters);

                return data.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> TolakDir(string nomor)
        {
            try
            {
                var qry = "usp_Fin_Revert_Doctrac_SPD_Dir";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@SPD_Nomor", nomor, DbType.String, ParameterDirection.Input);

                var data = await new DapperRepository<string>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryStoredProcedureAsync(qry, parameters);

                return data.FirstOrDefault();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<KaryawanInfo> GetInfoKaryawan(int id_ms_login)
        {
            try
            {
                var qryKaryawan = "select kar.ID_Ms_Karyawan, " +
                    "kar.ID_Ms_Divisi, " +
                    "kar.ID_Ms_Bagian, " +
                    "kar.ID_Ms_Jabatan, " +
                    "div.Nama as Divisi, " +
                    "bag.Nama as Bagian, " +
                    "usaha.Kode as PT, " +
                    "CASE " +
                    "WHEN bag.Nama = 'EST' or bag.Nama = 'FAC' or div.Nama = 'CS' THEN 'KIRIM' " +
                    "ELSE 'ALL' " +
                    "END as Role " +
                    "from Ms_Karyawan kar " +
                    "join Ms_Bagian bag on kar.ID_Ms_Bagian = bag.ID_Ms_Bagian " +
                    "join Ms_UnitUsaha usaha on kar.ID_Ms_UnitUsaha = usaha.ID_Ms_UnitUsaha " +
                    "join Ms_Divisi div on kar.ID_Ms_Divisi = div.ID_Ms_Divisi " +
                    "join Ms_Login log on kar.ID_Ms_Karyawan = log.ID_Ms_Karyawan " +
                    "where log.ID_Ms_Login = @p_id_ms_login; ";

                var dataKaryawan = await new DapperRepository<KaryawanInfo>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qryKaryawan, new { p_id_ms_login = id_ms_login });

           
                return dataKaryawan.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<OpsiTujuan>> GetOpsiTujuanEST()
        {
            try
            {
                var qry = "select DISTINCT doct.FlowType as value, doct.FlowType as label from Dt_DocFlowType doct " +
                    "join Dt_DocFlowSetting docs on doct.FlowType = docs.FlowType " +
                    "where doct.RowStatus != 'D' " +
                    "and (docs.Bagian = 'EST' or docs.Bagian = 'FAC') " +
                    "order by doct.FlowType asc; ";

                var data = await new DapperRepository<OpsiTujuan>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry);

                return data.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<List<OpsiTujuan>> GetOpsiTujuanJKT()
        {
            try
            {
                var qry = "select DISTINCT doct.FlowType as value, doct.FlowType as label from Dt_DocFlowType doct " +
                    "join Dt_DocFlowSetting docs on doct.FlowType = docs.FlowType " +
                    "where doct.RowStatus != 'D' " +
                    "and (docs.Bagian != 'EST' or docs.Bagian != 'FAC') " +
                    "order by doct.FlowType asc; ";

                var data = await new DapperRepository<OpsiTujuan>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry);

                return data.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Dt_DocStatus> GetDocStatusKirim(int id, string nomor)
        {
            try
            {
                var qry = "select * from Dt_DocStatus " +
                    "where ID = @p_id " +
                    "and Nomor = @p_nomor; ";

                var data = await new DapperRepository<Dt_DocStatus>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, new { p_id = id, p_nomor = nomor });

                return data.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Dt_DocStatus> GetDocStatus(int id)
        {
            try
            {
                var qry = "select * from Dt_DocStatus where ID = @p_id; ";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_id", id, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<Dt_DocStatus>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                return data.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dt_DocStatus GetDocStatusbyID(int id)
        {
            try
            {
                var qry = "select * from Dt_DocStatus " +
                    "where ID = @p_id; ";

                var data = new DapperRepository<Dt_DocStatus>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .Query(qry, new { p_id = id });

                return data.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetDivisi(int id_ms_login)
        {
            try
            {
                var qry = "select div.Nama as Divisi from Ms_Login lg " +
                    "join Ms_Karyawan kar on lg.ID_Ms_Karyawan = kar.ID_Ms_Karyawan " +
                    "join Ms_Divisi div on kar.ID_Ms_Divisi = div.ID_Ms_Divisi " +
                    "where lg.ID_Ms_Login = @p_id_ms_login; ";

                var data = new DapperRepository<string>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .Query(qry, new { p_id_ms_login = id_ms_login });

                return data.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<Dt_DocProcessStatus> GetDocProcessStatus(string nomor, string jenis)
        {
            try
            {
                var qry = "select * from Dt_DocProcessStatus " +
                    "where Nomor = @p_nomor " +
                    "and Jenis = @p_jenis " +
                    "and SendTime is null " +
                    "order by LastModifiedTime desc; ";

                var data = await new DapperRepository<Dt_DocProcessStatus>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, new { p_nomor = nomor, p_jenis = jenis });

                return data.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<Dt_DocFlowSetting> getDtDocFlowSetting(string flowtype, string bagian, string jenis, int status)
        {
            try
            {
                var qry = "select * from Dt_DocFlowSetting " +
                    "where FlowType = @p_flowtype " +
                    "and Bagian = @p_bagian " +
                    "and DocType = @p_jenis " +
                    "and Status = @p_status; ";

                var data = await new DapperRepository<Dt_DocFlowSetting>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, new { p_flowtype = flowtype, p_bagian = bagian, p_jenis = jenis, p_status = status });

                return data.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<Ms_MataUang> getMsMataUang()
        {
            try
            {
                var qry = "select * from Ms_MataUang; ";

                var data = new DapperRepository<Ms_MataUang>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .Query(qry);

                return data.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Fn_SPD getFnSPD(int id_fn_spd)
        {
            try
            {
                var qry = "select * from Fn_Spd " +
                    "where ID_Fn_SPD = @p_id_fn_spd " +
                    "and ModifyStatus != 'D'; ";

                var data = new DapperRepository<Fn_SPD>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .Query(qry, new { p_id_fn_spd = id_fn_spd });

                return data.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Fn_SPD> getFnSPDTolak(int refid)
        {
            try
            {
                var qry = "select * from Fn_Spd " +
                    "where ID_Fn_SPD = @p_refid; ";

                var data = await new DapperRepository<Fn_SPD>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, new { p_refid = refid });

                return data.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dt_DocDeliveryStatus getDocDeliveryStatus(string nomor, string jenis)
        {
            try
            {
                var qry = "select * from Dt_DocDeliveryStatus " +
                    "where Nomor = @p_nomor " +
                    "and Jenis = @p_jenis " +
                    "and ReceivedTime is null " +
                    "ORDER BY LastModifiedTime desc; ";

                var data = new DapperRepository<Dt_DocDeliveryStatus>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .Query(qry, new { p_nomor = nomor, p_jenis = jenis });

                return data.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Dt_DocFlowType> getFlow(string flowtype, string bagian)
        {
            try
            {
                var qry = "select * from Dt_DocFlowType doctype " +
                    "join Dt_DocFlowSetting docset on doctype.FlowType = docset.FlowType " +
                    "where docset.FlowType = @p_flowtype " +
                    "and docset.Bagian = @p_bagian; ";

                var data = await new DapperRepository<Dt_DocFlowType>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, new { p_flowtype = flowtype, p_bagian = bagian });

                return data.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<List<NoteList>> getDirNotes(string nomor)
        {
            try
            {
                var qry = "select log.Username, notes.Flag, notes.ModifiedDate, notes.Notes from Dt_Notes notes " +
                    "join Ms_Login log on notes.ModifiedBy = log.ID_Ms_Login " +
                    "where notes.Referensi = @p_nomor " +
                    "and notes.Form = 'SPD' " +
                    "and notes.Flag = 'SPD - Reject - Direktur'; ";


                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_nomor", nomor, DbType.String, ParameterDirection.Input);

                var data = await new DapperRepository<NoteList>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                return data.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<SPDScan>> getSpdScan(int id_fn_spd)
        {
            try
            {
                var qry = "select * from Fn_SPD_Scan " +
                    "where ID_Fn_SPD = @p_id_fn_spd " +
                    "and ModifyStatus != 'D'; ";


                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_id_fn_spd", id_fn_spd, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<SPDScan>(_dbConnectionFactorySQL.GetDbConnection("SCAN_JVE"))
                    .QueryAsync(qry, parameters);

                return data.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Fn_SPD>> getCutOffSpdEstate(int refid)
        {
            try
            {
                var qry = "select * from Fn_Spd " +
                    "where ID_Fn_SPD = @p_refid " +
                    "and (Tanggal >= datefromparts(2020, 04, 13) and (ID_Ms_Bagian = 4 or ID_Ms_Bagian = 5)); ";


                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_refid", refid, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<Fn_SPD>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                return data.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Fn_SPD>> getCutOffSpdHO(int refid)
        {
            try
            {
                var qry = "select * from Fn_Spd " +
                    "where ID_Fn_SPD = @p_refid " +
                    "and (Tanggal >= datefromparts(2020, 06, 08) and (ID_Ms_Bagian != 4 or ID_Ms_Bagian != 5)); ";


                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_refid", refid, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<Fn_SPD>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                return data.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Fn_SPD_Count>> getDownloadedScanByDiv(int id_ms_bagian, int id_ms_divisi)
        {
            try
            {
                var qry = "select * from Fn_SPD_Count " +
                    "where ID_Ms_Bagian = @p_id_ms_bagian " +
                    "and ID_Ms_Divisi = @p_id_ms_divisi; ";


                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_id_ms_bagian", id_ms_bagian, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@p_id_ms_divisi", id_ms_divisi, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<Fn_SPD_Count>(_dbConnectionFactorySQL.GetDbConnection("SCAN_JVE"))
                    .QueryAsync(qry, parameters);

                return data.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<DokumenSPD>> GetSpdDitolak(int id_ms_login)
        {
            try
            {
                var qry = "usp_Dt_GetDokumenDitolak";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@IdLogin", id_ms_login, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<DokumenSPD>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryStoredProcedureAsync(qry, parameters);

                return data.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
