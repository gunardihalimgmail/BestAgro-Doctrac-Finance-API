using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BestAgroCore.Common.Api;
using BestAgroCore.Common.Domain;
using BestAgroCore.Infrastructure.Data.EFRepositories.Contracts;
using Finance.Domain.Aggregate.ApprovalBkuBtu;
using Finance.Domain.DTO.BKU;
using Finance.Infrastructure;
using Finance.WebAPI.Application.Commands.ApprovalBKU;
using Finance.WebAPI.Application.Queries.ApprovalBKU;
using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Finance.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalBKUController : ControllerBase
    {
        private readonly ILogger<ApprovalBKUController> _logger;
        private readonly IApprovalBKUQueries _approvalBKUQueries;
        private readonly ICommandHandler<CreateUpdateApprovalBKUCommand> _createUpdateApprovalBKUCommand;
        private readonly IFn_BKU_CountRepository _Fn_BKU_CountRepository;
        private readonly IUnitOfWork<FinanceContext> _uow;
        private readonly IUnitOfWork<FinanceScanContext> _uowScan;


        public ApprovalBKUController(ILogger<ApprovalBKUController> logger, IApprovalBKUQueries approvalBKUQueries,
             ICommandHandler<CreateUpdateApprovalBKUCommand> createUpdateApprovalBKUCommand,
             IFn_BKU_CountRepository Fn_BKU_CountRepository, IUnitOfWork<FinanceContext> uow, IUnitOfWork<FinanceScanContext> uowScan
             ) 
        {
            _uow = uow;
            _uowScan = uowScan;
            _logger = logger;
            _approvalBKUQueries = approvalBKUQueries;
            _createUpdateApprovalBKUCommand = createUpdateApprovalBKUCommand;
            _Fn_BKU_CountRepository = Fn_BKU_CountRepository;

        }

        [HttpGet]
        [Route("getbkuappr/{id_ms_login}")]
        [Authorize]
        public async Task<IActionResult> GetListApprovalBKU(int id_ms_login)
        {
            try
            {
                #region start stopwatch
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                #endregion

                var dataApprovalBKUList = await _approvalBKUQueries.GetListApprovalBKU(id_ms_login);

                //foreach (var itemBKUList in dataApprovalBKUList)
                //{
                    //// cek apakah ada dokumen atau tidak
                    //var dataSPD = await _approvalBKUQueries.getBKUSPD(itemBKUList.ID);

                    //var IDFnSPDList = new List<int>();
                    //if (dataSPD.Any())
                    //{
                    //    foreach (var itemSPD in dataSPD.ToList())
                    //    {
                    //        int setIDSPD = Convert.ToInt32(itemSPD.ID_Fn_SPD);
                    //        IDFnSPDList.Add(setIDSPD);
                    //    }
                    //}

                    //var dataSPDScan = await _approvalBKUQueries.getSPDScan(IDFnSPDList);

                    //if (dataSPDScan.Any())
                    //{
                    //    itemBKUList.IsDownload = "1";
                    //}
                    //else
                    //{
                    //    itemBKUList.IsDownload = "0";
                    //}
                //}

                #region stop stopwatch
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine("RunTime Get List BKU: " + ts); //+ " id_ms_login: " + id_ms_login + " Pukul: " + DateTime.Now
                #endregion

                return Ok(new ApiOkResponse(dataApprovalBKUList));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get List BKU");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }

        [HttpGet]
        [Route("getdetailbku/{nomor}")]
        [Authorize]
        public async Task<IActionResult> GetDetailApprovalBKU(string nomor)
        {
            try
            {
                var result = new BKUDetailAndOPLPB();

                var replacedNomor = nomor.Replace("%2F", "/");
                var dataApprovalBKUList = await _approvalBKUQueries.GetDetailApprovalBKU(replacedNomor);
                var dataApprovalBKUOPLPBList = await _approvalBKUQueries.GetDetailApprovalBKUOPLPB(replacedNomor);
                var dataApprovalBKUHistoryList = await _approvalBKUQueries.GetDetailApprovalBKUHistory(replacedNomor);

                result.bkuDetail = dataApprovalBKUList;
                result.bkuOpLpbDetail = dataApprovalBKUOPLPBList;
                result.bkuDetailHistory = dataApprovalBKUHistoryList;

                return Ok(new ApiOkResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Detail BKU");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }


        [HttpGet]
        [Route("downloadbkuapprv/{id_fn_bku}/{id_ms_login}")]
        [Authorize]
        public async Task<IActionResult> DownloadMultipleFiles(int id_fn_bku, int id_ms_login)
        {
            try
            {
                var getDivisi = await _approvalBKUQueries.getDivisiAndJabatan(id_ms_login);
                var dataSPD = await _approvalBKUQueries.getBKUSPD(id_fn_bku);

                var IDFnSPDList = new List<int>();
                if (dataSPD.Any())
                {
                    foreach (var item in dataSPD.ToList())
                    {
                        int setIDSPD = Convert.ToInt32(item.ID_Fn_SPD);
                        IDFnSPDList.Add(setIDSPD);
                    }
                }

                // cek di Fn_BKU_Count

                var dataSPDScan = await _approvalBKUQueries.getSPDScan(IDFnSPDList);

                if (dataSPDScan.Any())
                {
                    int countScan = dataSPDScan.Count();
                    byte[] file = null;

                    if (countScan > 1)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                            {
                                //foreach (var file in byteArrayList)
                                foreach (var dataScans in dataSPDScan)
                                {
                                    file = dataScans.FileData;
                                    //var filename = scans.Nomor;
                                    //var ext = ".pdf";
                                    var entry = archive.CreateEntry(dataScans.FileName, CompressionLevel.Optimal);
                                    using (var zipStream = entry.Open())
                                    {
                                        zipStream.Write(file, 0, file.Length);
                                    }
                                }
                            }

                            // Insert ke fn_bku_count
                            try
                            {
                                Fn_BKU_Count countDownload = new Fn_BKU_Count();

                                countDownload.ID_Fn_BKU = id_fn_bku;
                                countDownload.ID_Ms_Divisi = getDivisi.ID_Ms_Divisi;
                                countDownload.ID_Ms_Bagian = getDivisi.ID_Ms_Bagian;
                                countDownload.ID_Ms_Jabatan = getDivisi.ID_Ms_Jabatan;
                                countDownload.ID_Ms_Login = id_ms_login;
                                countDownload.DownloadDate = DateTime.Now;

                                _Fn_BKU_CountRepository.Insert(countDownload);

                                await _uowScan.CommitAsync();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                            return File(ms.ToArray(), "application/zip", "Archive.zip");
                        }
                    }
                    else if (countScan == 1)
                    {
                        var qScanSPD = dataSPDScan.First();

                        string contentType = "application/pdf";

                        var cd = new System.Net.Mime.ContentDisposition
                        {
                            FileName = qScanSPD.FileName,
                            Inline = true,
                        };

                        Response.Headers["Content-Disposition"] = cd.ToString();

                        // Insert ke fn_bku_count
                        try
                        {
                            Fn_BKU_Count countDownload = new Fn_BKU_Count();

                            countDownload.ID_Fn_BKU = id_fn_bku;
                            countDownload.ID_Ms_Divisi = getDivisi.ID_Ms_Divisi;
                            countDownload.ID_Ms_Bagian = getDivisi.ID_Ms_Bagian;
                            countDownload.ID_Ms_Jabatan = getDivisi.ID_Ms_Jabatan;
                            countDownload.ID_Ms_Login = id_ms_login;
                            countDownload.DownloadDate = DateTime.Now;

                            _Fn_BKU_CountRepository.Insert(countDownload);

                            await _uowScan.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        return File(qScanSPD.FileData, contentType, qScanSPD.FileName);
                    }
                }

                return Ok(new ApiOkResponse(200));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Download Approval BKU");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }


        [HttpPost]
        [Route("getcheckdownloaddoc")]
        [Authorize]
        public async Task<IActionResult> GetCheckDownloadApprovalBKU([FromBody] nomorDokumen request)
        {
            try
            {
                var result = "";
                var getDivisi = await _approvalBKUQueries.getDivisiAndJabatan(request.ID_Ms_Login);

                // Divisi CEO / DIR tetap harus cek / download dokumen
                List<int> ListBKUCollection = new List<int>();
                List<int> ListBKUCollectionDownloaded = new List<int>();

                foreach (var item in request.docKey)
                {
                    var fullFormatNomor = ConvertToNomor(item);

                    if (fullFormatNomor.Contains("BKU") || fullFormatNomor.Contains("KKU"))
                    {
                        int idBku = 0;

                        try
                        {
                            var getIdBKU = await _approvalBKUQueries.getIdBku(fullFormatNomor);

                            if (getIdBKU == null)
                            {
                                idBku = 0;
                            }
                            else
                            {
                                idBku = getIdBKU.ID_Fn_BKU;
                            }


                        }
                        catch
                        {
                            idBku = 0;
                        }

                        if (idBku > 0)
                        {
                            ListBKUCollection.Add(idBku);
                        }
                    }
                }

                int countBKUCollection = ListBKUCollection.Count();
                int countBKUDownloaded = 0;

                try
                {

                    var GetBKUDownloaded = await _approvalBKUQueries.getFnBkuCount(ListBKUCollection, getDivisi.ID_Ms_Divisi, getDivisi.ID_Ms_Bagian, getDivisi.ID_Ms_Jabatan);

                    if (GetBKUDownloaded.Any())
                    {
                        foreach (var item in GetBKUDownloaded)
                        {
                            int id = Convert.ToInt32(item.ID_Fn_BKU);
                            ListBKUCollectionDownloaded.Add(id);
                        }
                    }

                    countBKUDownloaded = GetBKUDownloaded.Count();
                }
                catch
                {
                    countBKUDownloaded = 0;
                }


                if (countBKUDownloaded < countBKUCollection)
                {
                    string errorDownload = "";

                    var getExceptBKUID = ListBKUCollection.Except(ListBKUCollectionDownloaded);

                    if (getExceptBKUID.Any())
                    {
                        foreach (var item in getExceptBKUID)
                        {
                            int myID = Convert.ToInt32(item);

                            var nomorBKUException = await _approvalBKUQueries.getNomor(myID);

                            if (errorDownload == "")
                            {
                                errorDownload = nomorBKUException.Nomor;
                            }
                            else
                            {
                                errorDownload += ", " + nomorBKUException.Nomor;
                            }
                        }
                    }

                    //return "count";
                    result = errorDownload + " belum download dokumen.";
                }
                else
                {
                    result = "ReadyToApprove";
                }

                // Untuk Check Dokumen sudah ter Approve


                return Ok(new ApiOkResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Detail BKU");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }


        [HttpPost]
        [Route("approvedokumenbku")]
        [Authorize]
        public async Task<IActionResult> ApproveDokumenBKU([FromBody] CreateUpdateApprovalBKUCommand request)
        {
            // 6 7 SPV Finance
            // 2 3 4 Direktur
            // 1587 CEO

            try
            {

                var result = "ReadyToApprove";
                request.docKey = request.docKey.Distinct();
                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

                var DocKeyList = new List<string>();

                var dataApprovalBKUList = await _approvalBKUQueries.GetListApprovalBKU(request.ID_Ms_Login); // request.ID_Ms_Login_Replacement == 0 ? request.ID_Ms_Login : request.ID_Ms_Login_Replacement

                for (int i = 0; i < dataApprovalBKUList.Count; i++)
                {
                    DocKeyList.Add(dataApprovalBKUList[i].DocKey);
                }

                var notInListBKUDoc = request.docKey.Except(DocKeyList).ToList();

                request.docKey = request.docKey.Intersect(DocKeyList);


                // Approve Dir dan CEO
                if (request.ID_Ms_Login == 2 || request.ID_Ms_Login == 3 || request.ID_Ms_Login == 4 || request.ID_Ms_Login == 1587)
                {
                    await _createUpdateApprovalBKUCommand.Handle(request, CancellationToken.None);
                    result = "ApprovedByDirektur";
                }


                // Google Authentication untuk approve Manager Finance atau BNC
                if (request.ApprovalFlag != "Reject")
                {


                    if (request.ID_Ms_Login == 57 || request.ID_Ms_Login == 93 || request.JabatanFlag.Contains("MANAGER")) // || request.ID_Ms_Login_Replacement == 57 || request.ID_Ms_Login_Replacement == 93
                    {
                        var getSecretKey = await _approvalBKUQueries.getAccountSecretKey(request.ID_Ms_Login);

                        if (getSecretKey != null)
                        {
                            bool isCorrectPIN = tfa.ValidateTwoFactorPIN(getSecretKey.AccountSecretKey, request.AuthCodeFromPhone);
                            result = isCorrectPIN ? "CorrectPin" : "WrongPin";
                        }
                    }


                    // Apabila true maka yg approve manager jika tikda maka yg approve SPV dengan id_ms_login 6, 7, 8, 19
                    // ||  request.ID_Ms_Login_Replacement == 6 || request.ID_Ms_Login_Replacement == 7 || request.ID_Ms_Login_Replacement == 8 || request.ID_Ms_Login_Replacement == 19
                    if (result == "CorrectPin" || request.ID_Ms_Login == 6 || request.ID_Ms_Login == 7 || request.ID_Ms_Login == 8 || request.ID_Ms_Login == 19 || request.JabatanFlag.Contains("SPV")) 
                    {

                        if (!request.docKey.Any())
                        {
                            return Ok(new ApiOkResponse("AllApproved"));
                        }
                        else
                        {
                            await _createUpdateApprovalBKUCommand.Handle(request, CancellationToken.None);

                            if (notInListBKUDoc.Count > 0)
                            {
                                var notinlistreturn = "";

                                foreach (var item in notInListBKUDoc)
                                {
                                    if (notinlistreturn == "")
                                    {
                                        notinlistreturn = item;
                                    }
                                    else
                                    {
                                        notinlistreturn += ", " + item;
                                    }
                                }

                                result = "ApprovedBySPVOrManager" + "|" + notinlistreturn;
                            }
                            else
                            {
                                result = "ApprovedBySPVOrManager";
                            }

                        }

                    }
                }

                if (request.ApprovalFlag == "Reject")
                {
                    await _createUpdateApprovalBKUCommand.Handle(request, CancellationToken.None);
                    result = "RejectedByManager";
                }

                return Ok(new ApiOkResponse(result));
            }
            catch (Exception ex)
            {
                if (ex.Message == "false")
                {
                    return Ok(new ApiOkResponse("Dokumen sudah direject"));
                }
                else
                {
                    _logger.LogCritical(ex, $"Error on Get Google Authentication");
                    return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
                }

            }
        }

        private string ConvertToNomor(string docKey)
        {
            // Convert docKey to Nomor
            string nomor = docKey;

            int length = nomor.Length;

            string fullNomor = "";
            string separator = "/";

            if (length == 17)
            {
                string bagian = nomor.Substring(0, 3);
                string pt = nomor.Substring(3, 2);
                string form = nomor.Substring(5, 3);

                string year = nomor.Substring(length - 9, 2);
                string month = nomor.Substring(length - 7, 2);
                string last5digits = nomor.Substring(length - 5, 5);

                fullNomor = bagian + separator + pt + separator + form + separator + year + separator + month + separator + last5digits;
            }
            else if (length == 18)
            {
                string bagian = nomor.Substring(0, 3);
                string pt = nomor.Substring(3, 3);
                string form = nomor.Substring(6, 3);

                string year = nomor.Substring(length - 9, 2);
                string month = nomor.Substring(length - 7, 2);
                string last5digits = nomor.Substring(length - 5, 5);

                fullNomor = bagian + separator + pt + separator + form + separator + year + separator + month + separator + last5digits;
            }

            return fullNomor;

        }

    }
}
