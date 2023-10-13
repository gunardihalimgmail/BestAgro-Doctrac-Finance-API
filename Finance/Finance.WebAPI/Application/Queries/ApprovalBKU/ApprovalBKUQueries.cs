using BestAgroCore.Common.Infrastructure.Data.Contracts;
using BestAgroCore.Infrastructure.Data.DapperRepositories;
using Dapper;
using Finance.Domain.Aggregate.ApprovalBkuBtu;
using Finance.Domain.Aggregate.User;
using Finance.Domain.DTO;
using Finance.Domain.DTO.BKU;
using Finance.Domain.DTO.SPD;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.WebAPI.Application.Queries.ApprovalBKU
{
    public class ApprovalBKUQueries : IApprovalBKUQueries
    {
        private readonly IDbSQLConectionFactory _dbConnectionFactorySQL;

        public ApprovalBKUQueries(IDbSQLConectionFactory dbConnectionFactorySQL)
        {
            _dbConnectionFactorySQL = dbConnectionFactorySQL;
        }

        public async Task<List<BKUList>> GetListApprovalBKU(int id_ms_login)
        {
            try
            {
                var qry = "";

                //if (id_ms_login == 2 || id_ms_login == 3 || id_ms_login == 4 || id_ms_login == 4350)
                //{
                //    qry = "usp_Dt_GetApprovalBkuDir";
                //}
                //else
                //{
                //    qry = "usp_Dt_GetApprovalBku";
                //}

                //DynamicParameters parameters = new DynamicParameters();

                //parameters.Add("@IdLogin", id_ms_login, DbType.Int32, ParameterDirection.Input);

                //var data = await new DapperRepository<BKUList>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                //    .QueryStoredProcedureAsync(qry, parameters);

                var qry_rev = "";
                if (id_ms_login == 2 || id_ms_login == 3 || id_ms_login == 4 || id_ms_login == 4350)
                {
                    qry_rev = "exec usp_Dt_GetApprovalBkuDir " + id_ms_login;
                    var data = await new DapperRepository<BKUList>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                        .QueryAsync(qry_rev);
                    return data.ToList();
                }
                else
                {
                    qry_rev = "exec usp_Dt_GetApprovalBku " + id_ms_login;
                    var data = await new DapperRepository<BKUList>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                        .QueryAsync(qry_rev);
                    return data.ToList();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<BKUDetail>> GetDetailApprovalBKU(string replacedNomor)
        {
            try
            {
                var qry = "usp_Dt_GetDetailBku";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@nomor", replacedNomor, DbType.String, ParameterDirection.Input, replacedNomor.Length);

                var data = await new DapperRepository<BKUDetail>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryStoredProcedureAsync(qry, parameters);

                return data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<BKUOPLPBDetail>> GetDetailApprovalBKUOPLPB(string replacedNomor)
        {
            try
            {
                var qry = "usp_Dt_GetDetailBKULPB";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@nomor", replacedNomor, DbType.String, ParameterDirection.Input, replacedNomor.Length);

                var data = await new DapperRepository<BKUOPLPBDetail>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryStoredProcedureAsync(qry, parameters);

                return data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BKUApprovalDetailHistory> GetDetailApprovalBKUHistory(string replacedNomor)
        {
            try
            {
                var qry = "usp_Dt_GetDetailBkuHistoryApproval";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@nomor", replacedNomor, DbType.String, ParameterDirection.Input, replacedNomor.Length);

                var data = await new DapperRepository<BKUApprovalDetailHistory>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryStoredProcedureAsync(qry, parameters);

                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<BKUSPD>> getBKUSPD(int id_fn_bku)
        {
            try
            {
                var qry = "select bku.Nomor, bkuspd.ID_Fn_SPD from Fn_BKU bku " +
                    "join Fn_BKU_SPD bkuspd on (bku.ID_Fn_BKU = bkuspd.ID_Fn_BKU and bkuspd.ModifyStatus != 'D') " +
                    "where bku.ID_Fn_BKU = @p_id_fn_bku " +
                    "and bku.ModifyStatus != 'D';";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_id_fn_bku", id_fn_bku, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<BKUSPD>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                return data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<SPDScan>> getSPDScan(List<int> id_fn_spd)
        {
            try
            {
                var qry = "select * from Fn_SPD_Scan  " +
                    "where ID_Fn_SPD in @p_id_fn_spd " +
                    "and ModifyStatus != 'D'; ";

                var data = await new DapperRepository<SPDScan>(_dbConnectionFactorySQL.GetDbConnection("SCAN_JVE"))
                    .QueryAsync(qry, new { p_id_fn_spd = id_fn_spd.ToArray() });

                return data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DivisiAndJabatan> getDivisiAndJabatan(int id_ms_login)
        {
            try
            {
                var qry = " select kar.ID_Ms_Karyawan, " +
                    "kar.ID_Ms_Divisi, " +
                    "kar.ID_Ms_Bagian, " +
                    "kar.ID_Ms_Jabatan, " +
                    "jab.Nama as Jabatan, " +
                    "div.Nama as Divisi " +
                    "from Ms_Login log " +
                    "join Ms_Karyawan kar on log.ID_Ms_Karyawan = kar.ID_Ms_Karyawan " +
                    "join Ms_Jabatan jab on kar.ID_MS_Jabatan = jab.ID_MS_Jabatan " +
                    "join Ms_Divisi div on kar.ID_Ms_Divisi = div.ID_Ms_Divisi " +
                    "where log.ID_Ms_Login = @p_id_ms_login;";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_id_ms_login", id_ms_login, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<DivisiAndJabatan>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GoogleAuth> getAccountSecretKey(int id_ms_login)
        {
            try
            {
                var qry = " select auths.AccountSecretKey from Ms_Autentikasi_User authusr " +
                    "join Ms_Autentikasi auths on authusr.ID_Ms_Autentikasi = auths.ID_Ms_Autentikasi " +
                    "where authusr.ID_Ms_Login = @p_id_ms_login " +
                    "and authusr.ModifyStatus != 'D' " +
                    "and auths.ModifyStatus != 'D' " +
                    "and authusr.TanggalAwalEfektif <= CAST(CAST(GETDATE() AS date) AS datetime) " +
                    "and authusr.TanggalAkhirEfektif >= CAST(CAST(GETDATE() AS date) AS datetime); ";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_id_ms_login", id_ms_login, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<GoogleAuth>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Fn_BKU> getIdBku(string nomor)
        {
            try
            {
                var qry = " select bku.ID_Fn_BKU from Fn_BKU bku " +
                    "join Fn_BKU_SPD bkuspd on (bku.ID_Fn_BKU = bkuspd.ID_Fn_BKU and bkuspd.ModifyStatus != 'D') " +
                    "join Fn_Spd spd on(bkuspd.ID_Fn_SPD = spd.ID_Fn_SPD and (spd.Tanggal >= datefromparts(2020, 04, 13) and(spd.ID_Ms_Bagian = 4 or spd.ID_Ms_Bagian = 5))) " +
                    "where bku.Nomor = @p_nomor " +
                    "and bku.ModifyStatus != 'D'; ";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_nomor", nomor, DbType.String, ParameterDirection.Input, nomor.Length);

                var data = await new DapperRepository<Fn_BKU>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Fn_BKU_Count>> getFnBkuCount(List<int> id_fn_bku, int id_ms_divisi, int id_ms_bagian, int id_ms_jabatan)
        {
            try
            {
                var qry = "select * from Fn_BKU_Count " +
                    "where ID_Fn_BKU in @p_id_fn_bku " +
                    "and ID_Ms_Bagian = @p_id_ms_bagian " +
                    "and ID_Ms_Divisi = @p_id_ms_divisi " +
                    "and ID_Ms_Jabatan = @p_id_ms_jabatan; ";

                var data = await new DapperRepository<Fn_BKU_Count>(_dbConnectionFactorySQL.GetDbConnection("SCAN_JVE"))
                    .QueryAsync(qry, new
                    {
                        p_id_fn_bku = id_fn_bku.ToArray(),
                        p_id_ms_divisi = id_ms_divisi,
                        p_id_ms_bagian = id_ms_bagian,
                        p_id_ms_jabatan = id_ms_jabatan
                    });

                return data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Fn_BKU> getNomor(int id_fn_bku)
        {
            try
            {
                var qry = " select * from Fn_BKU " +
                    "where ID_Fn_BKU = @p_id_fn_bku;";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_id_fn_bku", id_fn_bku, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<Fn_BKU>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Ms_Autentikasi> getMsAutentikasi(int id_ms_login)
        {
            try
            {
                var qry = " select auths.* from Ms_Autentikasi_User authusr " +
                    "join Ms_Autentikasi auths on authusr.ID_Ms_Autentikasi = auths.ID_Ms_Autentikasi " +
                    "where authusr.ID_Ms_Login = @p_id_ms_login " +
                    "and authusr.TanggalAwalEfektif <= CAST(CAST(GETDATE() AS date) AS datetime) " +
                    "and authusr.TanggalAkhirEfektif >= CAST(CAST(GETDATE() AS date) AS datetime); ";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_id_ms_login", id_ms_login, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<Ms_Autentikasi>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Fn_BKU> getIDBKU(string nomor)
        {
            try
            {
                var qry = "select * from Fn_BKU " +
                    "where Nomor = @p_nomor " +
                    "and ModifyStatus != 'D'; ";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_nomor", nomor, DbType.String, ParameterDirection.Input, nomor.Length);

                var data = await new DapperRepository<Fn_BKU>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Fn_BTU> getIDBTU(string nomor)
        {
            try
            {
                var qry = "select * from Fn_BTU " +
                    "where Nomor = @p_nomor " +
                    "and ModifyStatus != 'D'; ";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_nomor", nomor, DbType.String, ParameterDirection.Input, nomor.Length);

                var data = await new DapperRepository<Fn_BTU>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Dt_ApprovalBkuBtu> approvalBKU(int id_fn_bku)
        {
            try
            {
                var qry = "select * from Dt_ApprovalBkuBtu " +
                    "where Form = 9 " +
                    "and FormId = @p_id_fn_bku " +
                    "and RowStatus != 'D'; ";

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_id_fn_bku", id_fn_bku, DbType.Int32, ParameterDirection.Input);

                var data = await new DapperRepository<Dt_ApprovalBkuBtu>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, new { p_id_fn_bku = id_fn_bku });

                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Dt_ApprovalBkuBtu> approvalBTU(int id_fn_btu)
        {
            try
            {
                var qry = "select * from Dt_ApprovalBkuBtu " +
                    "where Form = 10 " +
                    "and FormId = @p_id_fn_btu " +
                    "and RowStatus != 'D'; ";

                var data = await new DapperRepository<Dt_ApprovalBkuBtu>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, new { p_id_fn_btu = id_fn_btu });

                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Dt_ApprovalBkuBtu> getApprovedBKU(string docKey)
        {
            try
            {
                var qry = "select StatusAppr3 from Dt_ApprovalBkuBtu dtapprv " +
                    "join Fn_BKU fnbku on dtapprv.FormId = fnbku.ID_Fn_BKU " +
                    "where REPLACE(fnbku.Nomor, '/', '') = 'JKTB1BKU181000317' ; ";


                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@p_dockey", docKey, DbType.Int32, ParameterDirection.Input);


                var data = await new DapperRepository<Dt_ApprovalBkuBtu>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, parameters);

                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
