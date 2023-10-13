using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BestAgroCore.Common.Api;
using BestAgroCore.Common.Domain;
using BestAgroCore.Infrastructure.Data.DapperRepositories;
using BestAgroCore.Infrastructure.Data.EFRepositories.Contracts;
using Dapper;
using Finance.Domain.Aggregate.ApprovalSPD;
using Finance.Domain.DTO.SPD;
using Finance.Domain.DTO.SPD.ListApprovalSPD;
using Finance.Domain.DTO.User;
using Finance.Infrastructure;
using Finance.WebAPI.Application.Commands.ApprovalSPD;
using Finance.WebAPI.Application.Queries.ApprovalSPD;
using Finance.WebAPI.Application.Queries.ListHistoryApprovalSPD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Finance.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalSPDController : ControllerBase
    {
        private readonly ILogger<ApprovalSPDController> _logger;
        private readonly IApprovalSPDQueries _approvalSPDQueries;
        private readonly ICommandHandler<CreateKirimSPDCommand> _createKirimSPDCommand;
        private readonly ICommandHandler<CreateTerimaSPDCommand> _createTerimaSPDCommand;
        private readonly ICommandHandler<CreateTolakSPDCommand> _createTolakSPDCommand;
        private readonly IFn_SPD_CountRepository _Fn_SPD_CountRepository;
        private readonly IUnitOfWork<FinanceScanContext> _uowScan;
        private readonly IListHistoryApprovalSPDQueries _listApprovalSPDQueries;
        //private readonly ICommandHandler<CreateKirimSPDESTCommand> _createKirimSPDESTCommand;
        //private readonly ICommandHandler<CreateKirimSPDHOCommand> _createKirimSPDHOCommand;
        //private readonly ICommandHandler<CreateCommentCommand> _createCommentCommand;
        //private readonly ICommandHandler<UpdateDitujukanCommand> _updateDitujukanCommand;


        // ICommandHandler<UpdateDitujukanCommand> UpdateDitujukanCommand
        // ICommandHandler<CreateCommentCommand> CreateCommentCommand
        // ICommandHandler<CreateKirimSPDESTCommand> CreateKirimSPDESTCommand 
        // ICommandHandler<CreateKirimSPDHOCommand> createKirimSPDHOCommand
        public ApprovalSPDController(ILogger<ApprovalSPDController> logger, IApprovalSPDQueries approvalSPDQueries,
            ICommandHandler<CreateKirimSPDCommand> createKirimSPDCommand, ICommandHandler<CreateTerimaSPDCommand> CreateTerimaSPDCommand, 
            ICommandHandler<CreateTolakSPDCommand> CreateTolakSPDCommand, IFn_SPD_CountRepository Fn_SPD_CountRepository, 
            IUnitOfWork<FinanceScanContext> uowScan, IListHistoryApprovalSPDQueries listApprovalSPDQueries)
        {
            _logger = logger;
            _approvalSPDQueries = approvalSPDQueries;
            _createKirimSPDCommand = createKirimSPDCommand;
            _createTerimaSPDCommand = CreateTerimaSPDCommand;
            _createTolakSPDCommand = CreateTolakSPDCommand;
            _Fn_SPD_CountRepository = Fn_SPD_CountRepository;
            _uowScan = uowScan;
            _listApprovalSPDQueries = listApprovalSPDQueries;

            //_createKirimSPDESTCommand = CreateKirimSPDESTCommand;
            //_createKirimSPDHOCommand = createKirimSPDHOCommand;
            //_createCommentCommand = CreateCommentCommand;
            //_updateDitujukanCommand = UpdateDitujukanCommand;

        }


        [HttpGet]
        [Route("gettokirim/{id_ms_login}")]
        [Authorize]
        public async Task<IActionResult> GetKirimSPD(int id_ms_login)
        {
            try
            {
                var result = new ResultDataListKirimTerima();

                var dataKaryawan = await _approvalSPDQueries.GetInfoKaryawan(id_ms_login);
                var dataToKirim = await _approvalSPDQueries.GetToKirim(id_ms_login, dataKaryawan.Bagian, dataKaryawan.Divisi, dataKaryawan.PT);

                result.DataKaryawan = dataKaryawan;
                result.SpdKirimTerimaList = dataToKirim;


                return Ok(new ApiOkResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Data To Kirim");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }


        [HttpGet]
        [Route("getkirimantidakditerima/{id_ms_login}")]
        [Authorize]
        public async Task<IActionResult> GetKirimanTidakDiterima(int id_ms_login)
        {
            try
            {
                var dataToKirim = await _approvalSPDQueries.GetKirimNotTerima(id_ms_login);

                return Ok(new ApiOkResponse(dataToKirim));

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Data To Kirim");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }


        [HttpGet]
        [Route("gettoterima/{id_ms_login}")]
        [Authorize]
        public async Task<IActionResult> GetTerimaSPD(int id_ms_login)
        {
            try
            {
                var result = new ResultDataListKirimTerima();

                var dataKaryawan = await _approvalSPDQueries.GetInfoKaryawan(id_ms_login);
                var dataToTerima = await _approvalSPDQueries.GetToTerima(id_ms_login);

                foreach (var item in dataToTerima)
                {
                    var qScanSPD = await _approvalSPDQueries.getSpdScan(item.RefId);

                    if (qScanSPD.Any())
                    {
                        item.IsDownload = "1";
                    }
                    else
                    {
                        item.IsDownload = "0";
                    }
                }

                result.DataKaryawan = dataKaryawan;
                result.SpdKirimTerimaList = dataToTerima;


                return Ok(new ApiOkResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Data To Terima");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }


        [HttpGet]
        [Route("opsitujuanlist/{bagian}")]
        [Authorize]
        public async Task<IActionResult> OpsiTujuanList(string bagian)
        {
            try
            {
                var listTujuan = new List<OpsiTujuan>();

                if (bagian == "JKT")
                {
                    var qFlowType = await _approvalSPDQueries.GetOpsiTujuanJKT();

                    listTujuan = qFlowType.ToList();
                }
                else if (bagian == "EST")//case user Kebun
                {
                    var qFlowType = await _approvalSPDQueries.GetOpsiTujuanEST();

                    listTujuan = qFlowType.ToList();
                }


                return Ok(new ApiOkResponse(listTujuan));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Data Opsi Tujuan");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }

        [HttpGet]
        [Route("getrejectnotedir/{nomor}")]
        [Authorize]
        public async Task<IActionResult> GetComment(string nomor)
        {
            try
            {
                var replacedNomor = nomor.Replace("%2F", "/");

                var listDoc = new List<NoteList>();

                var getComment = await _approvalSPDQueries.getDirNotes(replacedNomor);

                if (getComment.Any())
                {
                    foreach (var item in getComment.ToList())
                    {
                        NoteList singleNotes = new NoteList();

                        singleNotes.Flag = item.Flag;
                        singleNotes.Notes = item.Username + " - " + item.ModifiedDate.ToString("dd/MM/yyyy") + " - " + item.Notes;

                        listDoc.Add(singleNotes);
                    }
                }
                else
                {
                    NoteList singleNotes = new NoteList();

                    singleNotes.Flag = "";
                    singleNotes.Notes = "";

                    listDoc.Add(singleNotes);
                }

                return Ok(new ApiOkResponse(listDoc));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Data Opsi Tujuan");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }


        [HttpPost]
        [Route("kirimspd")]
        [Authorize]
        public async Task<IActionResult> KirimDokumenSPD([FromBody] CreateKirimSPDCommand request)
        {

            try
            {
                var srtringValidation = "";
                foreach (var item in request.dokumenKirimTerima)
                {
                    var sBagian = item.Bagian;
                    var sFlowType = item.FlowType;

                    var rFlowIsExist = await _approvalSPDQueries.getFlow(item.FlowType, item.Bagian);
                    if (rFlowIsExist == null)
                    {
                        srtringValidation += "SPD " + sBagian + " dengan Nomor " + item.Nomor
                            + " tidak dapat di tujukan " + sFlowType + "\n";
                    }
                }

                if (srtringValidation != "")
                {
                    return Ok(new ApiOkResponse(srtringValidation));
                }
                else
                {
                    await _createKirimSPDCommand.Handle(request, CancellationToken.None);
                    return Ok(new ApiOkResponse(200));
                }


            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on kirim SPD");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }


        [HttpGet]
        [Route("downloadspdscn/{id_fn_spd}/{id_ms_login}")]
        [Authorize]
        public async Task<IActionResult> DownloadFile(int id_fn_spd, int id_ms_login)
        {
            try
            {
                var infoKaryawan = await _approvalSPDQueries.GetInfoKaryawan(id_ms_login);
                var qScanSPD = await _approvalSPDQueries.getSpdScan(id_fn_spd);
                var dataSPDScan = qScanSPD.First();

                if (qScanSPD != null)
                {
                    // contentType = MimeType.GetMimeType(filedata, filename); // find the mime type of a file based on the file signature not the extension
                    var contentType = "application/pdf";

                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = dataSPDScan.FileName,
                        Inline = true,
                    };

                    Response.Headers["Content-Disposition"] = cd.ToString();

                    // Insert ke fn_bku_count
                    try
                    {
                        Fn_SPD_Count single = new Fn_SPD_Count();
                        single.ID_Fn_SPD = id_fn_spd;
                        single.ID_Ms_Login = id_ms_login;
                        single.ID_Ms_Bagian = infoKaryawan.ID_Ms_Bagian;
                        single.ID_Ms_Divisi = infoKaryawan.ID_Ms_Divisi;
                        single.DownloadDate = DateTime.Now;

                        _Fn_SPD_CountRepository.Insert(single);

                        await _uowScan.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    return File(dataSPDScan.FileData, contentType, dataSPDScan.FileName);

                }
                else
                {
                    return new EmptyResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Download SPD");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }


        [HttpPost]
        [Route("terimaspd")]
        [Authorize]
        public async Task<IActionResult> TerimaDokumenSPD([FromBody] CreateTerimaSPDCommand request)
        {

            try
            {
                string message = "";

                if (request.id_ms_login == 2 || request.id_ms_login == 3 || request.id_ms_login == 4 || request.id_ms_login == 1587)
                {
                    await _createTerimaSPDCommand.Handle(request, CancellationToken.None);
                }
                else
                {
                    var infoKaryawan = await _approvalSPDQueries.GetInfoKaryawan(request.id_ms_login);

                    var reqDok = new List<string>();
                    var scannedDok = new List<string>();
                    //compare
                    foreach (var item in request.dokumenKirimTerima)
                    {
                        if (item.Bagian == "EST" || item.Bagian == "FAC")
                        {
                            // Cut Off SPD Tanggal 2020 04 13 Start di Upload, Dibawah Tanggal SPD di Skip
                            var GetCutOffSPDEstate = await _approvalSPDQueries.getCutOffSpdEstate(item.RefId);

                            if (GetCutOffSPDEstate.Any())
                            {
                                reqDok.Add(item.RefId.ToString());
                            }
                        }
                        else // SPD HO wajib download
                        {
                            // Cut Off SPD HO Tanggal 2020 06 08
                            var GetCutOffSPDHO = await _approvalSPDQueries.getCutOffSpdHO(item.RefId);

                            if (GetCutOffSPDHO.Any())
                            {
                                reqDok.Add(item.RefId.ToString());
                            }
                        }
                    }

                    var downloadedScan = await _approvalSPDQueries.getDownloadedScanByDiv(infoKaryawan.ID_Ms_Bagian, infoKaryawan.ID_Ms_Divisi);

                    foreach (var item in downloadedScan)
                    {

                        scannedDok.Add(item.ID_Fn_SPD.ToString());
                    }

                    var unScannedDok = reqDok.Except(scannedDok).ToList();

                    if (infoKaryawan.ID_Ms_Divisi == 16) //* FINE X HARUS DOWNLOAD ULANG
                    {
                        unScannedDok = new List<string>(); //* set as empty list
                    }


                    if (unScannedDok.Any())
                    {
                        string unDownloadDok = "";
                        foreach (var id in unScannedDok)
                        {
                            var dok = request.dokumenKirimTerima.Where(x => x.RefId.ToString() == id).First();
                            if (unDownloadDok != "")
                            {
                                unDownloadDok = unDownloadDok + "\n" + dok.Nomor;
                            }
                            else
                            {
                                unDownloadDok = dok.Nomor;
                            }
                        }
                        message = "Mohon download dokumen fisik SPD terlebih dahulu." + "\n" + "Nomor SPD: " + "\n" + unDownloadDok;
                    }
                    else
                    {
                        await _createTerimaSPDCommand.Handle(request, CancellationToken.None);
                    }
                }


                if (message == "")
                {
                    return Ok(new ApiOkResponse(200));
                }
                else
                {
                    return Ok(new ApiOkResponse(message));
                }


            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Terima SPD");
                return Ok(new ApiBadRequestResponse(500, "Something Wrong"));

            }
        }

        [HttpPost]
        [Route("tolakspd")]
        [Authorize]
        public async Task<IActionResult> TolakDokumenSPD([FromBody] CreateTolakSPDCommand request)
        {

            try
            {

                await _createTolakSPDCommand.Handle(request, CancellationToken.None);
                return Ok(new ApiOkResponse(200));

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Tolak SPD");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));

            }
        }


        [HttpPost]
        [Route("getlistdokumen")]
        public async Task<IActionResult> GetListDokumen([FromBody] GetListDokumenCommand request)
        {
            try
            {
                string startdate = request.Startdate;
                string enddate = request.Enddate;
                int id_ms_login = request.id_ms_login;
                string keyword = request.Keyword;
                string pt = string.Join(",", request.Pt);

                DateTime start = new DateTime(Convert.ToInt32(startdate.Substring(0, 4)),
                    Convert.ToInt32(startdate.Substring(4, 2)),
                    Convert.ToInt32(startdate.Substring(6, 2)));

                DateTime end = new DateTime(Convert.ToInt32(enddate.Substring(0, 4)),
                    Convert.ToInt32(enddate.Substring(4, 2)),
                    Convert.ToInt32(enddate.Substring(6, 2)));

                List<HistoryApprovalSPD> listDoc = new List<HistoryApprovalSPD>();

                UserDetail qGet = _listApprovalSPDQueries.GetUserInfo(id_ms_login).Result;

                bool isUserKebun = false;

                if (qGet != null)
                {
                    if (qGet.Bagian == "EST" || qGet.Bagian == "FAC")
                    {
                        // orang kebun hanya finance yang monitor list dokumen
                        if (qGet.Divisi != "FINE")
                        {
                            return null;
                        }

                        isUserKebun = true;
                    }
                    // utk HO, mau diatur di role?
                }
                else
                {
                    return null;
                }

                //string newKey = "";

                //if (keyword == "aaa")
                //    newKey = "";
                //else
                //{
                //string encodedKeyword = keyword;
                //newKey = HttpUtility.UrlDecode(encodedKeyword);
                //newKey = keyword;

                //}

                var data = await _listApprovalSPDQueries.GetListHistorySPD(id_ms_login, pt, keyword, start, end, isUserKebun);

                return Ok(new ApiOkResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Update Supplier Price");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }


        //[HttpGet]
        //[Route("getlistdokumen/{id_ms_login}/{pt}/{keyword}/{startdate}/{enddate}")]
        //public async Task<IActionResult> GetListDokumen(int id_ms_login, string pt, string keyword, string startdate, string enddate)
        //{
        //    try
        //    {
        //        DateTime start = new DateTime(Convert.ToInt32(startdate.Substring(0, 4)),
        //            Convert.ToInt32(startdate.Substring(4, 2)),
        //            Convert.ToInt32(startdate.Substring(6, 2)));

        //        DateTime end = new DateTime(Convert.ToInt32(enddate.Substring(0, 4)),
        //            Convert.ToInt32(enddate.Substring(4, 2)),
        //            Convert.ToInt32(enddate.Substring(6, 2)));

        //        List<HistoryApprovalSPD> listDoc = new List<HistoryApprovalSPD>();

        //        UserDetail qGet = _listApprovalSPDQueries.GetUserInfo(id_ms_login).Result;

        //        bool isUserKebun = false;

        //        if (qGet != null)
        //        {
        //            if (qGet.Bagian == "EST" || qGet.Bagian == "FAC")
        //            {
        //                // orang kebun hanya finance yang monitor list dokumen
        //                if (qGet.Divisi != "FINE")
        //                {
        //                    return null;
        //                }

        //                isUserKebun = true;
        //            }
        //            // utk HO, mau diatur di role?
        //        }
        //        else
        //        {
        //            return null;
        //        }

        //        string newKey = "";

        //        if (keyword == "aaa")
        //            newKey = "";
        //        else
        //        {
        //            string encodedKeyword = keyword;
        //            newKey = HttpUtility.UrlDecode(encodedKeyword);
        //        }

        //        var data = await _listApprovalSPDQueries.GetListHistorySPD(id_ms_login, pt, newKey, start, end, isUserKebun);

        //        return Ok(new ApiOkResponse(data));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogCritical(ex, $"Error on Update Supplier Price");
        //        return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
        //    }
        //}

        [HttpGet]
        [Route("getdokflowstatus/{id}")]
        public async Task<IActionResult> GetDocFlowStatus(int id)
        {
            try
            {
                string nomor = _listApprovalSPDQueries.GetNomorSPDByID(id).Result;

                var data = await _listApprovalSPDQueries.GetListHistorySPDDetail(nomor);

                return Ok(new ApiOkResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Update Supplier Price");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }


        [HttpGet]
        [Route("getdokflowstatusspd/{jenis}/{nomor}")]
        public async Task<IActionResult> GetDocFlowStatusSPD(string jenis, string nomor)
        {
            try
            {

                var replacedNomor = nomor.Replace("%2F", "/");

                var data = await _listApprovalSPDQueries.getdokflowstatus(jenis, replacedNomor);

                return Ok(new ApiOkResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Update Supplier Price");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }



        [HttpGet]
        [Route("getditolak/{id_ms_login}")]
        [Authorize]
        public async Task<IActionResult> GetSPDDitolak(int id_ms_login)
        {
            try
            {
                var result = new ResultDataListKirimTerima();

                var dataKaryawan = await _approvalSPDQueries.GetInfoKaryawan(id_ms_login);

                if (dataKaryawan.Bagian == "EST" || dataKaryawan.Bagian == "FAC")
                {
                    if (dataKaryawan.Divisi == "FINE")
                    {
                        var dataToKirim = await _approvalSPDQueries.GetSpdDitolak(id_ms_login);
                        result.SpdKirimTerimaList = dataToKirim;
                        result.DataKaryawan = dataKaryawan;

                        return Ok(new ApiOkResponse(result));
                    }
                    else
                    {
                        return Ok(new ApiOkResponse(result));
                    }
                }
                else
                {
                    return Ok(new ApiOkResponse(result));
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Data To Kirim");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }


        //[HttpPost]
        //[Route("kirimspdfromho")]
        //[Authorize]
        //public async Task<IActionResult> KirimDokumenSPDHO([FromBody] CreateKirimSPDHOCommand request)
        //{

        //    try
        //    {

        //        await _createKirimSPDHOCommand.Handle(request, CancellationToken.None);
        //        return Ok(new ApiOkResponse(200));

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogCritical(ex, $"Error on kirim SPD");
        //        return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
        //    }
        //}

        //[HttpPost]
        //[Route("kirimspdfromest")]
        //[Authorize]
        //public async Task<IActionResult> KirimDokumenSPDEST([FromBody] CreateKirimSPDESTCommand request)
        //{

        //    try
        //    {
        //        var srtringValidation = "";
        //        foreach (var item in request.dokumenKirimTerima)
        //        {
        //            var sBagian = item.Bagian;
        //            var sFlowType = item.FlowType;

        //            var rFlowIsExist = await _approvalSPDQueries.getFlow(item.FlowType, item.Bagian);
        //            if (rFlowIsExist == null)
        //            {
        //                srtringValidation += "SPD " + sBagian + " dengan Nomor " + item.Nomor
        //                    + " tidak dapat di tujukan " + sFlowType + "\n";
        //            }
        //        }

        //        if (srtringValidation != "")
        //        {
        //            return Ok(new ApiOkResponse(srtringValidation));
        //        }
        //        else
        //        {
        //            await _createKirimSPDESTCommand.Handle(request, CancellationToken.None);
        //            return Ok(new ApiOkResponse(200));
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogCritical(ex, $"Error on kirim SPD");
        //        return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
        //    }
        //}

        //[HttpPost]
        //[Route("simpancomment")]
        //[Authorize]
        //public async Task<IActionResult> simpanComment([FromBody] CreateCommentCommand request)
        //{

        //    try
        //    {

        //        await _createCommentCommand.Handle(request, CancellationToken.None);
        //        return Ok(new ApiOkResponse(200));


        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogCritical(ex, $"Error on kirim SPD");
        //        return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
        //    }
        //}


        //[HttpPost]
        //[Route("updateditujukan")]
        //[Authorize]
        //public async Task<IActionResult> updateDitujukan([FromBody] UpdateDitujukanCommand request)
        //{

        //    try
        //    {

        //        await _updateDitujukanCommand.Handle(request, CancellationToken.None);
        //        return Ok(new ApiOkResponse(200));


        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogCritical(ex, $"Error on update ditujukan");
        //        return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
        //    }
        //}


    }
}
