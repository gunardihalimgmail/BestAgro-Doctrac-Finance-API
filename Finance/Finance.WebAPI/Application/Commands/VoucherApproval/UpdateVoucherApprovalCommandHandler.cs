using AutoMapper;
using BestAgroCore.Common.Domain;
using BestAgroCore.Infrastructure.Data.EFRepositories.Contracts;
using Finance.Domain.Aggregate.VoucherApproval;
using Finance.Infrastructure;
using Finance.WebAPI.Application.Queries;
using Finance.Domain.DTO.VoucherApproval.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finance.Domain.DTO.VoucherApproval;
using Finance.WebAPI.Application.Queries.VoucherApproval;

namespace Finance.WebAPI.Application.Commands.VoucherApproval
{
    public class UpdateVoucherApprovalCommandHandler : ICommandHandler<UpdateVoucherApprovalCommand>
    {
        private readonly IUnitOfWork<FinanceContext> _uow;
        //private readonly IMapper _mapper;
        private readonly IFn_RealisasiVoucher_ApprovalRepository _fn_RealisasiVoucher_ApprovalRepository;
        private readonly IFn_RealisasiVoucherRepository _fn_RealisasiVoucherRepository;
        private readonly IFn_RealisasiVoucher_DetailRepository _fn_RealisasiVoucher_DetailRepository;
        private readonly IVoucherApprovalQueries _voucherApprovalQueries;


        public UpdateVoucherApprovalCommandHandler(IUnitOfWork<FinanceContext> uow,
            IFn_RealisasiVoucher_ApprovalRepository fn_RealisasiVoucher_ApprovalRepository,
            IFn_RealisasiVoucherRepository fn_RealisasiVoucherRepository,
            IVoucherApprovalQueries voucherApprovalQueries) //,IMapper mapper
        {
            _uow = uow;
            //_mapper = mapper;
            _fn_RealisasiVoucher_ApprovalRepository = fn_RealisasiVoucher_ApprovalRepository;
            _fn_RealisasiVoucherRepository = fn_RealisasiVoucherRepository;
            _voucherApprovalQueries = voucherApprovalQueries;
        }

        public async Task Handle(UpdateVoucherApprovalCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var dataApvVous = new List<VoucherRealisasiDetail>();
                DateTime now = DateTime.Now;
                DateTime tanggalrealisasi = command.tanggalrealisasi.Date;
                string str_tanggalrealisasi = tanggalrealisasi.ToString("yyyy-MM-dd");
                int modifyUser = command.lastmodifiedby;

                var cekRoleUser = await _voucherApprovalQueries.GetRoleVoucher(modifyUser);
                if (cekRoleUser != null && !String.IsNullOrEmpty(cekRoleUser))
                {
                    if (cekRoleUser == "RILIS")
                    {
                        var dataVouApv = await _voucherApprovalQueries.GetSingleVouApv(str_tanggalrealisasi);
                        if (dataVouApv != null)
                        {
                            if (dataVouApv.Id_Appr1 == 0 || dataVouApv.Id_Appr1 == null)
                            {
                                dataVouApv.LastModifiedBy = modifyUser;
                                dataVouApv.LastModifiedTime = now;
                                _fn_RealisasiVoucher_ApprovalRepository.Update(dataVouApv);
                            }
                        }
                        else
                        {
                            Fn_RealisasiVoucher_Approval insertVouApv = new Fn_RealisasiVoucher_Approval();
                            insertVouApv.Keterangan = "REALISASI VOUCHER, TANGGAL GIRO " + tanggalrealisasi.ToString("dd/MM/yyyy");
                            insertVouApv.Status = 0;
                            insertVouApv.TanggalRealisasi = tanggalrealisasi;
                            insertVouApv.LastModifiedBy = modifyUser;
                            insertVouApv.LastModifiedTime = now;
                            _fn_RealisasiVoucher_ApprovalRepository.Insert(insertVouApv);
                        }
                        var dataVouReals = await _voucherApprovalQueries.GetVouRealSPV(modifyUser, str_tanggalrealisasi);
                        foreach (var updateVouReal in dataVouReals)
                        {
                            updateVouReal.StatusRelease = "Y";
                            updateVouReal.LastModifiedBy = modifyUser;
                            updateVouReal.LastModifiedTime = now;
                            _fn_RealisasiVoucherRepository.Update(updateVouReal);
                        }
                    }
                    else if (cekRoleUser == "APPROVE MGR")
                    {
                        var dataVouApv = await _voucherApprovalQueries.GetSingleVouApv(str_tanggalrealisasi);
                        if (dataVouApv != null)
                        {
                            if (dataVouApv.Id_Appr1 == 0 || dataVouApv.Id_Appr1 == null)
                            {
                                dataVouApv.Id_Appr1 = modifyUser;
                                dataVouApv.TanggalAppr1 = now;
                                dataVouApv.Status = dataVouApv.Status + 1;
                                dataVouApv.LastModifiedBy = modifyUser;
                                dataVouApv.LastModifiedTime = now;
                                _fn_RealisasiVoucher_ApprovalRepository.Update(dataVouApv);
                            }
                        }
                    }
                    else if (cekRoleUser == "APPROVE DIR")
                    {
                        var dataVouApv = await _voucherApprovalQueries.GetSingleVouApv(str_tanggalrealisasi);
                        if (dataVouApv.Id_Appr2 == 0 || dataVouApv.Id_Appr2 == null)
                        {
                            dataVouApv.Id_Appr2 = modifyUser;
                            dataVouApv.TanggalAppr2 = now;
                            dataVouApv.Status = dataVouApv.Status + 1;
                            dataVouApv.LastModifiedBy = modifyUser;
                            dataVouApv.LastModifiedTime = now;
                            _fn_RealisasiVoucher_ApprovalRepository.Update(dataVouApv);
                        }
                    }
                    else if (cekRoleUser == "APPROVE CEO")
                    {
                        var dataVouApv = await _voucherApprovalQueries.GetSingleVouApv(str_tanggalrealisasi);
                        if (dataVouApv.Id_Appr3 == 0 || dataVouApv.Id_Appr3 == null)
                        {
                            dataVouApv.Id_Appr3 = modifyUser;
                            dataVouApv.TanggalAppr3 = now;
                            dataVouApv.Status = dataVouApv.Status + 1;
                            dataVouApv.LastModifiedBy = modifyUser;
                            dataVouApv.LastModifiedTime = now;
                            _fn_RealisasiVoucher_ApprovalRepository.Update(dataVouApv);
                        }
                    }
                }
                else
                {
                    HandleError("Voucher hanya bisa di approve oleh Manager / Direktur / CEO!");
                }

                await _uow.CommitAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string HandleError(string status)
        {
            throw new Exception(status);
        }

    }
}
