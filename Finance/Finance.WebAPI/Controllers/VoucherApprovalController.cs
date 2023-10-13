using System;
using System.Threading;
using System.Threading.Tasks;
using Finance.WebAPI.Application.Commands.VoucherApproval;
using Finance.WebAPI.Application.Queries;
using BestAgroCore.Common.Api;
using BestAgroCore.Common.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Finance.Domain.DTO.VoucherApproval.Request;
using Finance.WebAPI.Application.Queries.VoucherApproval;

namespace Finance.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherApprovalController : ControllerBase
    {
        private readonly ILogger<VoucherApprovalController> _logger;
        private readonly IVoucherApprovalQueries _voucherApprovalQueries;
        private readonly ICommandHandler<CreateVoucherApprovalCommand> _createVoucherApprovalCommand;
        private readonly ICommandHandler<UpdateVoucherApprovalCommand> _updateVoucherApprovalCommand;

        public VoucherApprovalController(ILogger<VoucherApprovalController> logger,
            ICommandHandler<CreateVoucherApprovalCommand> createVoucherApprovalCommand,
            ICommandHandler<UpdateVoucherApprovalCommand> updateVoucherApprovalCommand,
            IVoucherApprovalQueries voucherApprovalQueries)
        {
            _logger = logger;
            _voucherApprovalQueries = voucherApprovalQueries;
            _createVoucherApprovalCommand = createVoucherApprovalCommand;
            _updateVoucherApprovalCommand = updateVoucherApprovalCommand;
        }

        [HttpGet]
        [Route("getlistvoubku/{id_ms_login}/{tanggal}/{pt}")]
        public async Task<IActionResult> GetListVoucherBKU(int id_ms_login, string tanggal, string pt)
        {
            try
            {
                #region start stopwatch
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                #endregion

                var data = await _voucherApprovalQueries.GetVoucherBKU(id_ms_login, tanggal, pt);

                #region stop stopwatch
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine("RunTime Get List Voucher BKU: " + ts);
                #endregion

                return Ok(new ApiOkResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get List Voucher BKU");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }

        [HttpGet]
        [Route("getvouapv/{id_ms_login}")]
        public async Task<IActionResult> GetVoucherApproval(int id_ms_login)
        {
            try
            {
                #region start stopwatch
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                #endregion

                var role = await _voucherApprovalQueries.GetRoleVoucher(id_ms_login);

                var data = new object();
                if (role == "RILIS")
                {
                    data = await _voucherApprovalQueries.GetVoucherToReleasedSPV(id_ms_login);
                }
                else if (role == "APPROVE MGR")
                {
                    data = await _voucherApprovalQueries.GetVoucherToApproveMGR();
                }
                else if (role == "APPROVE DIR")
                {
                    data = await _voucherApprovalQueries.GetVoucherToApproveDIR();
                }
                else if (role == "APPROVE CEO")
                {
                    data = await _voucherApprovalQueries.GetVoucherToApproveDIR();
                }

                #region stop stopwatch
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine("RunTime Get Is Voucher Ready : " + ts);
                #endregion
                return Ok(new ApiOkResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Is Voucher Ready");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }

        [HttpGet]
        [Route("getvouapvdetail/{id_ms_login}/{tanggal}")]
        public async Task<IActionResult> GetVoucherApprovalDetail(int id_ms_login, string tanggal)
        {
            try
            {
                #region start stopwatch
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                #endregion

                var role = await _voucherApprovalQueries.GetRoleVoucher(id_ms_login);

                var data = new object();
                if (role == "RILIS")
                {
                    data = await _voucherApprovalQueries.GetVoucherDetailSPV(id_ms_login, tanggal);
                }
                else if (role.Contains("APPROVE"))
                {
                    data = await _voucherApprovalQueries.GetVoucherDetailMGMT(id_ms_login, tanggal);
                }

                #region stop stopwatch
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine("RunTime Get Is Voucher Ready : " + ts);
                #endregion

                return Ok(new ApiOkResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Is Voucher Ready");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }

        [HttpGet]
        [Route("getvourole/{id_ms_login}")]
        public async Task<IActionResult> GetVoucherRole(int id_ms_login)
        {
            try
            {
                #region start stopwatch
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                #endregion

                string rtn = "";

                var role = await _voucherApprovalQueries.GetRoleVoucher(id_ms_login);
                if (role.Contains("APPROVE")
                    && role != "APPROVE MGR")
                {
                    rtn = "MGMT";
                }
                else if (role == "APPROVE MGR")
                {
                    rtn = "MGR";
                }
                else if (role == "RILIS")
                {
                    rtn = "SPV";
                }

                #region stop stopwatch
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine("RunTime Get Is Voucher Ready : " + ts);
                #endregion

                return Ok(new ApiOkResponse(rtn));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Is Voucher Ready");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }

        [HttpGet]
        [Route("getvoupt/{id_ms_login}/{tanggal}")]
        public async Task<IActionResult> GetVoucherLembar(int id_ms_login, string tanggal)
        {
            try
            {
                #region start stopwatch
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                #endregion

                var data = new object();
                var role = await _voucherApprovalQueries.GetRoleVoucher(id_ms_login);
                if (role == "RILIS")
                {
                    data = await _voucherApprovalQueries.GetAllVoucherPTSPV(id_ms_login, tanggal);
                }
                else if (role.Contains("APPROVE"))
                {
                    data = await _voucherApprovalQueries.GetAllVoucherPT(id_ms_login, tanggal);
                }

                #region stop stopwatch
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine("RunTime Get Is Voucher Ready : " + ts);
                #endregion

                return Ok(new ApiOkResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Is Voucher Ready");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }

        [HttpGet]
        [Route("getvouapvlembar/{id_ms_login}/{tanggal}/{pt}")]
        public async Task<IActionResult> GetVoucherPT(int id_ms_login, string tanggal, string pt)
        {
            try
            {
                #region start stopwatch
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                #endregion

                var type = pt;
                var role = await _voucherApprovalQueries.GetRoleVoucher(id_ms_login);

                var data = new object();
                if (type == "ALL")
                {
                    if (role == "RILIS")
                    {
                        data = await _voucherApprovalQueries.GetAllPTVoucherLembarSPV(id_ms_login, tanggal);
                    }
                    else if (role.Contains("APPROVE"))
                    {
                        data = await _voucherApprovalQueries.GetAllPTVoucherLembarMGMT(id_ms_login, tanggal);
                    }
                }
                else
                {
                    if (role == "RILIS")
                    {
                        data = await _voucherApprovalQueries.GetSinglePTVoucherLembarSPV(id_ms_login, tanggal, pt);
                    }
                }



                #region stop stopwatch
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine("RunTime Get Is Voucher Ready : " + ts);
                #endregion

                return Ok(new ApiOkResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Is Voucher Ready");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }

        [HttpGet]
        [Route("getopsipt/{id_ms_login}/{tanggal}")]
        public async Task<IActionResult> GetOpsiPT(int id_ms_login, string tanggal)
        {
            try
            {
                #region start stopwatch
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                #endregion

                var role = await _voucherApprovalQueries.GetRoleVoucher(id_ms_login);

                var data = new object();
                if (role == "RILIS")
                {
                    data = await _voucherApprovalQueries.GetPTSPV(id_ms_login);
                }

                #region stop stopwatch
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine("RunTime Get Is Voucher Ready : " + ts); //+ " id_ms_login: " + id_ms_login + " Pukul: " + DateTime.Now
                #endregion

                return Ok(new ApiOkResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Is Voucher Ready");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }

        [HttpPost]
        [Route("createvoudtl")]
        [Authorize]
        public async Task<IActionResult> CreateVouDetail([FromBody] CreateVoucherApprovalCommand request)
        {
            try
            {
                var data = new object();

                int id_ms_login = request.lastmodifiedby;
                var role = await _voucherApprovalQueries.GetRoleVoucher(id_ms_login);
                if (role == "RILIS")
                {
                    string str_tanggalrealisasi = request.tanggalrealisasi.ToString("yyyy-MM-dd");
                    var vouApv = await _voucherApprovalQueries.GetSingleVouApv(str_tanggalrealisasi);
                    if (vouApv == null ||
                        (vouApv.Id_Appr1 == null || vouApv.Id_Appr1 == 0))
                    {
                        await _createVoucherApprovalCommand.Handle(request, CancellationToken.None);
                        data = "";
                    }
                    else
                    {
                        data = "Data Sudah Pernah Disimpan dan Diproses Manager Finance!";
                    }
                }

                return Ok(new ApiOkResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Bidding List");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }

        [HttpPost]
        [Route("rilisvouapv")]
        [Authorize]
        public async Task<IActionResult> RilisVouApv([FromBody] UpdateVoucherApprovalCommand request)
        {
            try
            {
                var data = new object();

                int id_ms_login = request.lastmodifiedby;
                var role = await _voucherApprovalQueries.GetRoleVoucher(id_ms_login);
                if (role == "RILIS")
                {
                    string str_tanggalrealisasi = request.tanggalrealisasi.ToString("yyyy-MM-dd");
                    var vouApv = await _voucherApprovalQueries.GetSingleVouApv(str_tanggalrealisasi);
                    if (vouApv == null ||
                        (vouApv.Id_Appr1 == null || vouApv.Id_Appr1 == 0))
                    {
                        await _updateVoucherApprovalCommand.Handle(request, CancellationToken.None);
                        data = "";
                    }
                    else
                    {
                        data = "Data Sudah Pernah Disimpan dan Diproses Manager Finance!";
                    }
                }
                else if (role.Contains("APPROVE"))
                {
                    await _updateVoucherApprovalCommand.Handle(request, CancellationToken.None);
                    data = "";
                }

                return Ok(new ApiOkResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Bidding List");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }

        [HttpGet]
        [Route("isready/{id_ms_login}/{tanggal}")]
        public async Task<IActionResult> IsReadyVouApv(int id_ms_login, string tanggal)
        {
            try
            {
                #region start stopwatch
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                #endregion
                var role = await _voucherApprovalQueries.GetRoleVoucher(id_ms_login);

                var data = new object();
                if (role == "RILIS")
                {
                    data = await _voucherApprovalQueries.GetIsReadyToReleased(id_ms_login, tanggal);
                }
                else if (role.Contains("APPROVE"))
                {
                    data = await _voucherApprovalQueries.GetIsReadyToApproved(tanggal);
                }

                #region stop stopwatch
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine("RunTime Get Is Voucher Ready : " + ts);
                #endregion

                return Ok(new ApiOkResponse(data));

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error on Get Bidding List");
                return BadRequest(new ApiBadRequestResponse(500, "Something Wrong"));
            }
        }

    }
}
