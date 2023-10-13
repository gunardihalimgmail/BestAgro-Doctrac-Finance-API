using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BestAgroCore.Common.Infrastructure.Data.Contracts;
using BestAgroCore.Infrastructure.Data.DapperRepositories;
using Finance.Domain.Aggregate.VoucherApproval;
using Finance.Domain.DTO.VoucherApproval;
using Finance.Domain.DTO.VoucherApproval.Request;
using Finance.Domain.Aggregate.Common;
using System.Globalization;
using Finance.WebAPI.Application.Queries.VoucherApproval;

namespace Finance.WebAPI.Application.Queries.VoucherApproval
{
    public class VoucherApprovalQueries : IVoucherApprovalQueries
    {
        private readonly IDbSQLConectionFactory _dbConnectionFactorySQL;

        public VoucherApprovalQueries(IDbSQLConectionFactory dbConnectionFactorySQL)
        {
            _dbConnectionFactorySQL = dbConnectionFactorySQL;
        }

        public async Task<List<VoucherPT>> GetPTSPV(int id_ms_login)
        {
            try
            {
                var qryVoucherLembar = " SELECT DISTINCT  uu.ID_Ms_UnitUsaha AS id,  " +
                    " uu.Kode AS kodept,  " +
                    " uu.Nama AS namapt,  " +
                    " '' AS komentar " +
                    " FROM[JVE].[dbo].[Ms_UnitUsaha] uu " +
                    " JOIN[JVE].[dbo].[Ms_Keuangan] keu ON uu.ID_Ms_UnitUsaha = keu.ID_Ms_UnitUsaha " +
                    " JOIN[JVE].[dbo].[Ms_Keuangan_RptRealVoucher] keurv ON keu.ID_Ms_Keuangan = keurv.ID_Ms_Keuangan " +
                    " WHERE keurv.Id_Ms_Login_Spv = @p_id_ms_login; ";
                var voucherLembar = await new DapperRepository<VoucherPT>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                            .QueryAsync(qryVoucherLembar, new
                            {
                                p_id_ms_login = id_ms_login
                            });

                return voucherLembar.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<VoucherPT>> GetAllVoucherPT(int id_ms_login, string tanggal)
        {
            try
            {
                var statusrelease = "Y";

                var qryVoucherLembar = " SELECT uu.ID_Ms_UnitUsaha AS id, " +
                        " uu.Kode AS kodept, " +
                        " uu.Nama AS namapt, " +
                        " rv.Komentar AS komentar " +
                        " FROM dbo.Fn_RealisasiVoucher rv " +
                        " JOIN dbo.Ms_UnitUsaha uu ON rv.ID_Ms_UnitUsaha = uu.ID_Ms_UnitUsaha " +
                        " JOIN dbo.Fn_RealisasiVoucher_Detail dv ON rv.ID_Fn_RealisasiVoucher = dv.ID_Fn_RealisasiVoucher " +
                        " WHERE rv.ModifyStatus <> 'D' " +
                        " AND rv.TanggalRealisasi = @p_tanggal " +
                        " AND rv.StatusRelease = @p_statusrelease " +
                        " GROUP BY uu.ID_Ms_UnitUsaha, uu.Kode, uu.Nama, rv.Komentar " +
                        " ORDER BY uu.ID_Ms_UnitUsaha, uu.Kode, uu.Nama, rv.Komentar; ";

                var voucherLembar = await new DapperRepository<VoucherPT>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                            .QueryAsync(qryVoucherLembar, new
                            {
                                p_tanggal = tanggal,
                                p_id_ms_login = id_ms_login,
                                p_statusrelease = statusrelease
                            });

                return voucherLembar.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<VoucherPT>> GetAllVoucherPTSPV(int id_ms_login, string tanggal)
        {
            try
            {
                var statusrelease = "N";

                var qryVoucherLembar = " SELECT uu.ID_Ms_UnitUsaha AS id, " +
                        " uu.Kode AS kodept, " +
                        " uu.Nama AS namapt, " +
                        " rv.Komentar AS komentar " +
                        " FROM dbo.Fn_RealisasiVoucher rv " +
                        " JOIN dbo.Ms_UnitUsaha uu ON rv.ID_Ms_UnitUsaha = uu.ID_Ms_UnitUsaha " +
                        " JOIN dbo.Fn_RealisasiVoucher_Detail dv ON rv.ID_Fn_RealisasiVoucher = dv.ID_Fn_RealisasiVoucher " +
                        " WHERE rv.ModifyStatus <> 'D' " +
                        " AND rv.TanggalRealisasi = @p_tanggal " +
                        " AND rv.CreatedBy = @p_id_ms_login " +
                        " AND rv.StatusRelease = @p_statusrelease " +
                        " GROUP BY uu.ID_Ms_UnitUsaha, uu.Kode, uu.Nama, rv.Komentar " +
                        " ORDER BY uu.ID_Ms_UnitUsaha, uu.Kode, uu.Nama, rv.Komentar; ";

                var voucherLembar = await new DapperRepository<VoucherPT>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                            .QueryAsync(qryVoucherLembar, new
                            {
                                p_tanggal = tanggal,
                                p_id_ms_login = id_ms_login,
                                p_statusrelease = statusrelease
                            });

                return voucherLembar.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<VoucherLembar>> GetAllPTVoucherLembarMGMT(int id_ms_login, string tanggal)
        {
            try
            {
                var statusrelease = "Y";

                var qryVoucherLembar = " SELECT keu.ID_Ms_Keuangan as id, " +
                        " uu.ID_Ms_UnitUsaha AS id_ms_unitusaha, " +
                        " uu.Kode AS kodept, " +
                        " uu.Nama AS namapt, " +
                        " dv.NomorRekening as rekening, " +
                        " SUM(dv.Nominal) AS totalnominal, " +
                        " COUNT(dv.ID_Fn_RealisasiVoucher_Detail) AS totallembar " +
                        " FROM dbo.Fn_RealisasiVoucher rv " +
                        " JOIN dbo.Ms_UnitUsaha uu ON rv.ID_Ms_UnitUsaha = uu.ID_Ms_UnitUsaha " +
                        " JOIN dbo.Fn_RealisasiVoucher_Detail dv ON rv.ID_Fn_RealisasiVoucher = dv.ID_Fn_RealisasiVoucher " +
                        " JOIN dbo.Ms_Keuangan keu ON dv.NomorRekening = CONCAT(keu.Kode , ' - ' , keu.NamaNomorKeuangan) " +
                        " WHERE rv.ModifyStatus <> 'D' " +
                        " AND rv.TanggalRealisasi = @p_tanggal " +
                        " AND rv.StatusRelease = @p_statusrelease " +
                        " GROUP BY keu.ID_Ms_Keuangan, uu.ID_Ms_UnitUsaha, uu.Kode, uu.Nama, dv.NomorRekening " +
                        " ORDER BY keu.ID_Ms_Keuangan, uu.ID_Ms_UnitUsaha, uu.Kode, uu.Nama, dv.NomorRekening; ";

                var voucherLembar = await new DapperRepository<VoucherLembar>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                            .QueryAsync(qryVoucherLembar, new
                            {
                                p_tanggal = tanggal,
                                p_id_ms_login = id_ms_login,
                                p_statusrelease = statusrelease
                            });

                return voucherLembar.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<VoucherLembar>> GetAllPTVoucherLembarSPV(int id_ms_login, string tanggal)
        {
            try
            {
                var statusrelease = "N";

                var qryVoucherLembar = " SELECT keu.ID_Ms_Keuangan as id, " +
                        " uu.ID_Ms_UnitUsaha AS id_ms_unitusaha, " +
                        " uu.Kode AS kodept, " +
                        " uu.Nama AS namapt, " +
                        " dv.NomorRekening as rekening, " +
                        " SUM(dv.Nominal) AS totalnominal, " +
                        " CAST(COUNT(dv.ID_Fn_RealisasiVoucher_Detail) AS varchar) AS totallembar " +
                        " FROM dbo.Fn_RealisasiVoucher rv " +
                        " JOIN dbo.Ms_UnitUsaha uu ON rv.ID_Ms_UnitUsaha = uu.ID_Ms_UnitUsaha " +
                        " JOIN dbo.Fn_RealisasiVoucher_Detail dv ON rv.ID_Fn_RealisasiVoucher = dv.ID_Fn_RealisasiVoucher " +
                        " JOIN dbo.Ms_Keuangan keu ON dv.NomorRekening = CONCAT(keu.Kode , ' - ' , keu.NamaNomorKeuangan) " +
                        " WHERE rv.ModifyStatus <> 'D' " +
                        " AND rv.TanggalRealisasi = @p_tanggal " +
                        " AND rv.CreatedBy = @p_id_ms_login " +
                        " AND rv.StatusRelease = @p_statusrelease " +
                        " GROUP BY keu.ID_Ms_Keuangan, uu.ID_Ms_UnitUsaha, uu.Kode, uu.Nama, dv.NomorRekening " +
                        " ORDER BY keu.ID_Ms_Keuangan, uu.ID_Ms_UnitUsaha, uu.Kode, uu.Nama, dv.NomorRekening; ";

                var voucherLembar = await new DapperRepository<VoucherLembar>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                            .QueryAsync(qryVoucherLembar, new
                            {
                                p_tanggal = tanggal,
                                p_id_ms_login = id_ms_login,
                                p_statusrelease = statusrelease
                            });

                return voucherLembar.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<VoucherLembar>> GetSinglePTVoucherLembarSPV(int id_ms_login, string tanggal, string kode_unitusaha)
        {
            try
            {
                // ambil dari SP
                string flaginternal = "N";
                string bagian = "JKT";
                string baseon = "G"; // GIRO

                var qryVoucherBKU = " EXEC usp_Dt_ReportRealisasiVoucherBKU_Count @IdLogin = @p_id_ms_login, " +
                    "@Tanggal = @p_tanggal, " +
                    "@PT = @p_kodept, " +
                    "@Bagian = @p_kodebagian," +
                    "@FlagInternal = @p_flaginternal, " +
                    "@BaseOn = @p_baseon; ";
                var voucherBKU = await new DapperRepository<VoucherLembar>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                .QueryAsync(qryVoucherBKU, new
                {
                    p_id_ms_login = id_ms_login,
                    p_tanggal = tanggal,
                    p_kodept = kode_unitusaha,
                    p_kodebagian = bagian,
                    p_flaginternal = flaginternal,
                    p_baseon = baseon
                });

                return voucherBKU.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<VoucherRealisasiDetail>> GetVoucherBKU(int id_ms_login, string tanggal, string pt)
        {
            try
            {
                // ambil dari SP
                string flaginternal = "N";
                string bagian = "JKT";
                string baseon = "G"; // GIRO

                var qryVoucherBKU = " EXEC usp_Dt_ReportRealisasiVoucherBKU @IdLogin = @p_id_ms_login, " +
                    "@Tanggal = @p_tanggal, " +
                    "@PT = @p_kodept, " +
                    "@Bagian = @p_kodebagian," +
                    "@FlagInternal = @p_flaginternal, " +
                    "@BaseOn = @p_baseon; ";
                var voucherBKU = await new DapperRepository<VoucherRealisasiDetail>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                .QueryAsync(qryVoucherBKU, new
                {
                    p_id_ms_login = id_ms_login,
                    p_tanggal = tanggal,
                    p_kodept = pt,
                    p_kodebagian = bagian,
                    p_flaginternal = flaginternal,
                    p_baseon = baseon
                });

                return voucherBKU.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetRoleVoucher(int id_ms_login)
        {
            string rtn = "";

            int IDLogin = id_ms_login;

            if (IDLogin == 6 || IDLogin == 7) //6   SIERLI 7   YANYAN
            {
                rtn = "RILIS";
            }
            else if (IDLogin == 2)   // DIR
            {
                rtn = "APPROVE DIR";
            }
            else if (IDLogin == 1587)  // CEO
            {
                rtn = "APPROVE CEO";
            }
            else if (IDLogin == 93)  // MGR FIN
            {
                rtn = "APPROVE MGR";
            }

            return rtn;
        }

        public async Task<List<VoucherApproved>> GetVoucherToApproveMGR()
        {
            try
            {
                var qryApprovalRealisasi = " SELECT DISTINCT " +
                    "vouapv.ID_Fn_RealisasiVoucher_Approval AS id, " +
                    "FORMAT(vouapv.TanggalRealisasi, 'yyyy-MM-dd') AS tanggal, " +
                    "CONCAT('REALISASI VOUCHER, TANGGAL GIRO ', FORMAT(vouapv.TanggalRealisasi, 'dd/MM/yyyy')) AS keterangan, " +
                    "vouapv.TanggalAppr1 AS mgrapvtime " +
                    "FROM dbo.Fn_RealisasiVoucher_Approval vouapv " +
                    "WHERE vouapv.Status = 0 " +
                    "AND(vouapv.Id_Appr1 IS NULL OR vouapv.Id_Appr1 = 0); ";

                var approvalRealisasi = await new DapperRepository<VoucherApproved>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qryApprovalRealisasi);

                return approvalRealisasi.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<VoucherApproved>> GetVoucherToApproveDIR()
        {
            try
            {
                var qryApprovalRealisasi = " SELECT DISTINCT " +
                    "vouapv.ID_Fn_RealisasiVoucher_Approval AS id, " +
                    "FORMAT(vouapv.TanggalRealisasi, 'yyyy-MM-dd') AS tanggal, " +
                    "CONCAT('REALISASI VOUCHER, TANGGAL GIRO ', FORMAT(vouapv.TanggalRealisasi, 'dd/MM/yyyy')) AS keterangan, " +
                    "vouapv.TanggalAppr1  AS mgrapvtime " +
                    "FROM dbo.Fn_RealisasiVoucher_Approval vouapv " +
                    "JOIN dbo.Fn_RealisasiVoucher vou ON vouapv.TanggalRealisasi = vou.TanggalRealisasi " +
                    "JOIN dbo.Fn_RealisasiVoucher_Detail voudet ON vou.ID_Fn_RealisasiVoucher = voudet.ID_Fn_RealisasiVoucher " +
                    "WHERE vouapv.Status > 0 " +
                    "AND voudet.Nominal <= 40000000 " +
                    "AND(vouapv.Id_Appr2 IS NULL OR vouapv.Id_Appr2 = 0) " +
                    "AND(vouapv.Id_Appr1 IS NOT NULL AND vouapv.Id_Appr1 != 0); ";

                var approvalRealisasi = await new DapperRepository<VoucherApproved>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qryApprovalRealisasi);

                return approvalRealisasi.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<VoucherApproved>> GetVoucherToApproveCEO()
        {
            try
            {
                var qryApprovalRealisasi = " SELECT DISTINCT " +
                    "vouapv.ID_Fn_RealisasiVoucher_Approval AS id, " +
                    "FORMAT(vouapv.TanggalRealisasi, 'yyyy-MM-dd') AS tanggal, " +
                    "CONCAT('REALISASI VOUCHER, TANGGAL GIRO ', FORMAT(vouapv.TanggalRealisasi, 'dd/MM/yyyy')) AS keterangan, " +
                    "vouapv.TanggalAppr1 AS mgrapvtime " +
                    "FROM dbo.Fn_RealisasiVoucher_Approval vouapv " +
                    "WHERE vouapv.Status > 0 " +
                    "AND(vouapv.Id_Appr3 IS NULL OR vouapv.Id_Appr3 = 0) " +
                    "AND(vouapv.Id_Appr1 IS NOT NULL AND vouapv.Id_Appr1 != 0); ";

                var approvalRealisasi = await new DapperRepository<VoucherApproved>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qryApprovalRealisasi);

                return approvalRealisasi.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<VoucherApproved>> GetVoucherToReleasedSPV(int id_ms_login)
        {
            try
            {
                string statusrelease = "N";

                var qry = "SELECT DISTINCT " +
                        "row_number() OVER (ORDER BY vouapv.TanggalRealisasi) AS id, " +
                        "FORMAT(vouapv.TanggalRealisasi, 'yyyy-MM-dd') AS tanggal, " +
                        "CONCAT('REALISASI VOUCHER, TANGGAL GIRO ', FORMAT(vouapv.TanggalRealisasi, 'dd/MM/yyyy')) AS keterangan, " +
                        "'' AS mgrapvtime " +
                        "FROM dbo.Fn_RealisasiVoucher vouapv " +
                        "WHERE vouapv.CreatedBy = @p_id_ms_login " +
                        "AND vouapv.StatusRelease = @p_statusrelease " +
                        "GROUP BY vouapv.TanggalRealisasi; ";

                var data = await new DapperRepository<VoucherApproved>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                .QueryAsync(qry, new
                {
                    p_id_ms_login = id_ms_login,
                    p_statusrelease = statusrelease
                });

                return data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<VoucherRealisasiDetail>> GetVoucherDetailSPV(int id_ms_login, string tanggal)
        {
            try
            {
                string statusrelease = "N";

                var qry = "SELECT DISTINCT dt.ID_Fn_RealisasiVoucher_Detail AS id, " +
                    "uu.Kode AS unitusaha, " +
                    "dt.NomorRekening AS keuangan, " +
                    "rv.TanggalRealisasi AS tanggal, " +
                    "dt.NomorBKU AS bku, " +
                    "dt.NomorGiro AS giro, " +
                    "dt.NomorVoucher AS voucher, " +
                    "dt.Nominal AS nominal, " +
                    "dt.Kepada AS supplier, " +
                    "dt.Catatan AS keterangan " +
                    "FROM dbo.Fn_RealisasiVoucher_Detail dt " +
                    "JOIN dbo.Fn_RealisasiVoucher rv on dt.ID_Fn_RealisasiVoucher = rv.ID_Fn_RealisasiVoucher " +
                    "JOIN dbo.Ms_UnitUsaha uu on rv.ID_Ms_UnitUsaha = uu.ID_Ms_UnitUsaha " +
                    "WHERE rv.TanggalRealisasi = @p_tanggalrealisasi " +
                    "AND rv.StatusRelease = @p_statusrelease " +
                    "and rv.CreatedBy = @p_id_ms_login " +
                    "ORDER BY voucher; ";

                var data = await new DapperRepository<VoucherRealisasiDetail>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, new
                    {
                        p_tanggalrealisasi = tanggal,
                        p_statusrelease = statusrelease,
                        p_id_ms_login = id_ms_login
                    });

                return data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<VoucherRealisasiDetail>> GetVoucherDetailMGMT(int id_ms_login, string tanggal)
        {
            try
            {
                string statusrelease = "Y";

                // dir <= 40 jt
                // ceo 0-100 jt
                var qry = " SELECT DISTINCT dt.ID_Fn_RealisasiVoucher_Detail as id, " +
                    "uu.Kode as unitusaha, " +
                    "dt.NomorRekening AS keuangan, " +
                    "rv.TanggalRealisasi AS tanggal, " +
                    "dt.NomorBKU AS bku, " +
                    "dt.NomorGiro AS giro, " +
                    "dt.NomorVoucher AS voucher, " +
                    "dt.Nominal AS nominal, " +
                    "dt.Kepada AS supplier, " +
                    "dt.Catatan AS keterangan " +
                    "FROM dbo.Fn_RealisasiVoucher_Detail dt " +
                        "JOIN dbo.Fn_RealisasiVoucher rv on dt.ID_Fn_RealisasiVoucher = rv.ID_Fn_RealisasiVoucher " +
                        "JOIN dbo.Fn_RealisasiVoucher_Approval ra on rv.TanggalRealisasi = ra.TanggalRealisasi " +
                        "JOIN dbo.Ms_UnitUsaha uu on rv.ID_Ms_UnitUsaha = uu.ID_Ms_UnitUsaha " +
                        "WHERE rv.TanggalRealisasi = @p_tanggal " +
                        "AND rv.StatusRelease = @p_statusrelease " +
                        "AND(((@p_id_ms_login = 1587 OR(@p_id_ms_login = 2 AND dt.Nominal <= 40000000)) AND ra.Status > 0) " +
                        "OR(@p_id_ms_login = 93)) " +
                        "AND dt.Nominal <= 100000000 " +
                        "ORDER BY voucher; ";

                var data = await new DapperRepository<VoucherRealisasiDetail>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, new
                    {
                        p_tanggal = tanggal,
                        p_statusrelease = statusrelease,
                        p_id_ms_login = id_ms_login
                    });
                return data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetIsReadyToReleased(int id_ms_login, string tanggal)
        {
            try
            {
                string msg = "";
                string tanggalstr = DateTime.ParseExact(tanggal, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                        .ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                var qryMaster = " SELECT DISTINCT uu.ID_Ms_UnitUsaha AS id_ms_unitusaha, " +
                            " uu.Kode AS kode, " +
                            " uu.Nama AS nama " +
                            " FROM[JVE].[dbo].[Ms_Keuangan_RptRealVoucher] rrv " +
                            " JOIN[JVE].[dbo].[Ms_Keuangan] keu ON rrv.ID_Ms_Keuangan = keu.ID_Ms_Keuangan " +
                            " JOIN[JVE].[dbo].[Ms_UnitUsaha] uu ON keu.ID_Ms_UnitUsaha = uu.ID_Ms_UnitUsaha " +
                            " WHERE rrv.ModifyStatus <> 'D' " +
                            " AND uu.ID_Ms_UnitUsaha NOT IN(17, 18) " +
                            " AND rrv.Id_Ms_Login_Spv = @p_id_ms_Login;";

                var dataMaster = await new DapperRepository<VoucherUnitUsaha>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qryMaster, new
                    {
                        p_id_ms_login = id_ms_login
                    });

                var qryVoucher = "SELECT DISTINCT uu.ID_Ms_UnitUsaha AS id_ms_unitusaha, " +
                    "uu.Kode AS kode, " +
                    "uu.Nama AS nama " +
                    "FROM dbo.Fn_RealisasiVoucher v " +
                    "JOIN dbo.Ms_UnitUsaha uu ON v.ID_Ms_UnitUsaha = uu.ID_Ms_UnitUsaha " +
                    "WHERE v.TanggalRealisasi = @p_tanggal " +
                    "AND v.ModifyStatus <> 'D'; ";
                var dataVoucher = await new DapperRepository<VoucherUnitUsaha>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qryVoucher, new
                    {
                        p_tanggal = tanggal
                    });

                var listMaster = dataMaster.Select(c => c.kode).ToList();
                var listVoucher = dataVoucher.Select(c => c.kode).ToList();
                var unsubmitedVoucherUU = listMaster.Except(listVoucher.ToList()).ToList();

                if (unsubmitedVoucherUU.Any())
                {
                    string notPassedPT = string.Join(",", unsubmitedVoucherUU);
                    msg = "PT berikut belum masuk dalam Laporan Realisasi Voucher Tanggal " + tanggalstr + " : "
                           + notPassedPT
                           + "\n"
                           + "\n" + " Jika yakin untuk merilis, silahkan menekan tombol Rilis kembali.";
                }

                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetIsReadyToApproved(string tanggal)
        {
            try
            {

                string msg = "";
                string tanggalstr = DateTime.ParseExact(tanggal, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                        .ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                var qryMaster = "SELECT DISTINCT uu.ID_Ms_UnitUsaha AS id_ms_unitusaha, " +
                    "uu.Kode AS kode, " +
                    "uu.Nama AS nama " +
                    "FROM dbo.Ms_Keuangan_RptRealVoucher rrv " +
                    "JOIN dbo.Ms_Keuangan keu ON rrv.ID_Ms_Keuangan = keu.ID_Ms_Keuangan " +
                    "JOIN dbo.Ms_UnitUsaha uu ON keu.ID_Ms_UnitUsaha = uu.ID_Ms_UnitUsaha " +
                    "WHERE rrv.ModifyStatus <> 'D' " +
                    "AND uu.ID_Ms_UnitUsaha NOT IN (17, 18); ";

                var dataMaster = await new DapperRepository<VoucherUnitUsaha>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qryMaster);

                var qryVoucher = "SELECT DISTINCT uu.ID_Ms_UnitUsaha AS id_ms_unitusaha, " +
                    "uu.Kode AS kode, " +
                    "uu.Nama AS nama " +
                    "FROM dbo.Fn_RealisasiVoucher v " +
                    "JOIN dbo.Ms_UnitUsaha uu ON v.ID_Ms_UnitUsaha = uu.ID_Ms_UnitUsaha " +
                    "WHERE v.TanggalRealisasi = @p_tanggal " +
                    "AND v.StatusRelease = 'Y' " +
                    "AND v.ModifyStatus <> 'D'; ";

                var dataVoucher = await new DapperRepository<VoucherUnitUsaha>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qryVoucher, new
                    {
                        p_tanggal = tanggal
                    });

                var listMaster = dataMaster.Select(c => c.kode).ToList();
                var listVoucher = dataVoucher.Select(c => c.kode).ToList();

                var unsubmitedVoucherUU = listMaster.Except(listVoucher.ToList()).ToList();
                if (unsubmitedVoucherUU.Any())
                {
                    string notPassedPT = string.Join(",", unsubmitedVoucherUU);
                    msg = "PT berikut belum masuk dalam Laporan Realisasi Voucher Tanggal " + tanggalstr + " : "
                        + notPassedPT
                        + "\n"
                        + "\n" + " Jika yakin untuk meng-approve, silahkan menekan tombol Approve kembali.";

                }

                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Fn_RealisasiVoucher_Approval> GetSingleVouApv(string tanggal)
        {
            try
            {
                string msg = "";

                var qryVoucher = "SELECT v.ID_Fn_RealisasiVoucher_Approval AS ID_Fn_RealisasiVoucher_Approval,  " +
                        "v.TanggalRealisasi AS TanggalRealisasi, " +
                        "v.Keterangan AS Keterangan, " +
                        "v.Status AS Status, " +
                        "v.Id_Appr1 AS Id_Appr1, " +
                        "v.Id_Appr2 AS Id_Appr2, " +
                        "v.Id_Appr3 AS Id_Appr3, " +
                        "v.TanggalAppr1 AS TanggalAppr1, " +
                        "v.TanggalAppr2 AS TanggalAppr2, " +
                        "v.TanggalAppr3 AS TanggalAppr3, " +
                        "v.LastModifiedBy AS LastModifiedBy, " +
                        "v.LastModifiedTime AS LastModifiedTime " +
                        "FROM dbo.Fn_RealisasiVoucher_Approval v " +
                        "WHERE v.TanggalRealisasi = @p_tanggal ; ";

                var dataVoucher = await new DapperRepository<Fn_RealisasiVoucher_Approval>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qryVoucher, new
                    {
                        p_tanggal = tanggal
                    });

                return dataVoucher.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Fn_RealisasiVoucher>> GetVouRealSPV(int id_ms_login, string tanggal)
        {
            try
            {
                string statusrelease = "N";

                var qry = "SELECT DISTINCT vouapv.* " +
                        "FROM dbo.Fn_RealisasiVoucher vouapv " +
                        "WHERE vouapv.CreatedBy = @p_id_ms_login " +
                        "AND vouapv.StatusRelease = @p_statusrelease " +
                        "AND vouapv.TanggalRealisasi = @p_tanggal ; ";

                var data = await new DapperRepository<Fn_RealisasiVoucher>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                .QueryAsync(qry, new
                {
                    p_id_ms_login = id_ms_login,
                    p_statusrelease = statusrelease,
                    p_tanggal = tanggal
                });

                return data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Fn_RealisasiVoucher_Detail>> GetRemoveVoucherDetail(int id_ms_login, string tanggal, int id_ms_unitusaha)
        {
            try
            {

                var qry = "SELECT DISTINCT dt.* " +
                    "FROM dbo.Fn_RealisasiVoucher_Detail dt " +
                    "JOIN dbo.Fn_RealisasiVoucher rv on dt.ID_Fn_RealisasiVoucher = rv.ID_Fn_RealisasiVoucher " +
                    "JOIN dbo.Ms_UnitUsaha uu on rv.ID_Ms_UnitUsaha = uu.ID_Ms_UnitUsaha " +
                    "WHERE rv.TanggalRealisasi = @p_tanggalrealisasi " +
                    "AND rv.CreatedBy = @p_id_ms_login " +
                    "AND rv.ID_Ms_UnitUsaha = @p_id_ms_unitusaha; ";

                var data = await new DapperRepository<Fn_RealisasiVoucher_Detail>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, new
                    {
                        p_tanggalrealisasi = tanggal,
                        p_id_ms_login = id_ms_login,
                        p_id_ms_unitusaha = id_ms_unitusaha
                    });

                return data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<Fn_RealisasiVoucher> GetRemoveVoucher(int id_ms_login, string tanggal, int id_ms_unitusaha)
        {
            try
            {
                var qry = "SELECT TOP 1 rv.* " +
                    "FROM dbo.Fn_RealisasiVoucher rv  " +
                    "WHERE rv.TanggalRealisasi = @p_tanggalrealisasi " +
                    "AND rv.CreatedBy = @p_id_ms_login " +
                    "AND rv.ID_Ms_UnitUsaha = @p_id_ms_unitusaha; ";

                var data = await new DapperRepository<Fn_RealisasiVoucher>(_dbConnectionFactorySQL.GetDbConnection("JVE"))
                    .QueryAsync(qry, new
                    {
                        p_tanggalrealisasi = tanggal,
                        p_id_ms_login = id_ms_login,
                        p_id_ms_unitusaha = id_ms_unitusaha
                    });

                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
