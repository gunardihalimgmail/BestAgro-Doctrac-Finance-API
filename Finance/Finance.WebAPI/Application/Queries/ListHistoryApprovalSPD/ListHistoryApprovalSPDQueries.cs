using BestAgroCore.Common.Infrastructure.Data.Contracts;
using BestAgroCore.Infrastructure.Data.DapperRepositories;
using Dapper;
using Finance.Domain.DTO.SPD;
using Finance.Domain.DTO.SPD.ListApprovalSPD;
using Finance.Domain.DTO.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.WebAPI.Application.Queries.ListHistoryApprovalSPD
{
    public class ListHistoryApprovalSPDQueries : IListHistoryApprovalSPDQueries
    {
        private readonly IDbSQLConectionFactory _dbConnectionFactorySQL;

        public ListHistoryApprovalSPDQueries(IDbSQLConectionFactory dbConnectionFactorySQL)
        {
            _dbConnectionFactorySQL = dbConnectionFactorySQL;
        }

        public async Task<UserDetail> GetUserInfo(int idLogin)
        {
            try
            {
                var qry = "select isnull(b.Nama, 'EST') as bagian, d.Nama as divisi " +
                    "from Ms_Login l with(nolock) " +
                    "join Ms_karyawan k with(nolock) on l.ID_Ms_Karyawan = k.ID_Ms_Karyawan " +
                    "join Ms_Bagian b with(nolock) on k.ID_Ms_Bagian = b.ID_Ms_Bagian " +
                    "join Ms_Divisi d with(nolock) on k.ID_Ms_Divisi = d.ID_Ms_Divisi " +
                    "where l.ModifyStatus != 'D' and l.ID_Ms_Login = @p_idLogin";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_idLogin", idLogin, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<UserDetail>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                UserDetail userDetail = new UserDetail();

                userDetail.Bagian = data.SingleOrDefault().Bagian;
                userDetail.Divisi = data.SingleOrDefault().Divisi;

                return userDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<HistoryApprovalSPD>> GetListHistorySPD(int id_ms_login, string pt, string keyword,
            DateTime startdate, DateTime enddate, bool isuserkebun)
        {
            try
            {
                //var qry = "usp_Dt_GetListDokumen";

                //DynamicParameters parameters = new DynamicParameters();

                //string perihal = " ";
                //Console.WriteLine(startdate.ToString() + '\n' + enddate.ToString() + '\n' + keyword + '\n' + pt + '\n' + 
                //            perihal + '\n' + id_ms_login.ToString() + '\n' + isuserkebun.ToString());

                //parameters.Add("@RequestDateFrom", startdate, DbType.Date, ParameterDirection.Input);
                //parameters.Add("@RequestDateTo", enddate, DbType.Date, ParameterDirection.Input);
                //parameters.Add("@CsvKodePT", pt, DbType.String, ParameterDirection.Input, keyword.Length);
                //parameters.Add("@DokumenNo", keyword, DbType.String, ParameterDirection.Input);
                //parameters.Add("@Perihal", perihal, DbType.String, ParameterDirection.Input, keyword.Length);
                //parameters.Add("@IdLogin", id_ms_login, DbType.Int32, ParameterDirection.Input);
                //parameters.Add("@IsUserKebun", isuserkebun, DbType.Boolean, ParameterDirection.Input);

                //var data = await new DapperRepository<HistoryApprovalSPD>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                //    .QueryStoredProcedureAsync(qry, parameters);

                var qry_rev = "exec usp_Dt_GetListDokumen '" + startdate + "', '" + enddate + "', '" + keyword + "', '" + pt + "', ' ', " + id_ms_login + "," + isuserkebun;
                var data = await new DapperRepository<HistoryApprovalSPD>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry_rev);

                return data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetNomorSPDByID(int id)
        {
            try
            {
                var qry = "select Nomor from Dt_DocStatus with(nolock) where ID = @p_id";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_id", id, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<string>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                return data.SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<HistoryApprovalSPDDetail>> GetListHistorySPDDetail(string nomor)
        {
            try
            {
                var qry = "usp_Dt_GetDetailStatus";

                DynamicParameters parameters = new DynamicParameters();

                string jenis = "SPD";

                parameters.Add("@Jenis", jenis, DbType.String, ParameterDirection.Input);
                parameters.Add("@DocNo", nomor, DbType.String, ParameterDirection.Input);

                var data = await new DapperRepository<HistoryApprovalSPDDetail>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryStoredProcedureAsync(qry, parameters);

                return data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<DokumenFlowStatus>> getdokflowstatus(string jenis, string nomor)
        {
            try
            {
                var qry = "usp_Dt_GetDetailStatus";

                DynamicParameters parameters = new DynamicParameters();


                parameters.Add("@Jenis", jenis, DbType.String, ParameterDirection.Input);
                parameters.Add("@DocNo", nomor, DbType.String, ParameterDirection.Input);

                var data = await new DapperRepository<DokumenFlowStatus>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
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
