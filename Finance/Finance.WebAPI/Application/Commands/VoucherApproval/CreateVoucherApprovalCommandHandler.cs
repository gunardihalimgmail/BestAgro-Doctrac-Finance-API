using AutoMapper;
using BestAgroCore.Common.Domain;
using BestAgroCore.Infrastructure.Data.EFRepositories.Contracts;
using Finance.Domain.Aggregate.VoucherApproval;
using Finance.Infrastructure;
using Finance.WebAPI.Application.Queries;
using Finance.Domain.DTO.VoucherApproval.Request;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finance.WebAPI.Application.Queries.VoucherApproval;

namespace Finance.WebAPI.Application.Commands.VoucherApproval
{
    public class CreateVoucherApprovalCommandHandler : ICommandHandler<CreateVoucherApprovalCommand>
    {
        private readonly IUnitOfWork<FinanceContext> _uow;
        //private readonly IMapper _mapper;
        private readonly IFn_RealisasiVoucherRepository _fn_RealisasiVoucherRepository;
        private readonly IFn_RealisasiVoucher_DetailRepository _fn_RealisasiVoucher_DetailRepository;
        private readonly IVoucherApprovalQueries _voucherApprovalQueries;

        public CreateVoucherApprovalCommandHandler(IUnitOfWork<FinanceContext> uow,
            IFn_RealisasiVoucherRepository fn_RealisasiVoucherRepository,
            IFn_RealisasiVoucher_DetailRepository fn_RealisasiVoucher_DetailRepository,
            IVoucherApprovalQueries voucherApprovalQueries)  //,IMapper mapper
        {
            _uow = uow;
            //_mapper = mapper;
            _fn_RealisasiVoucherRepository = fn_RealisasiVoucherRepository;
            _fn_RealisasiVoucher_DetailRepository = fn_RealisasiVoucher_DetailRepository;
            _voucherApprovalQueries = voucherApprovalQueries;
        } 

        public async Task Handle(CreateVoucherApprovalCommand command, CancellationToken cancellationToken)
        {
            try
            {
                DateTime now = DateTime.Now;
                DateTime tanggalrealisasi = command.tanggalrealisasi.Date;
                int modifyUser = command.createdby;
                int id_ms_unitusaha = command.id_ms_unitusaha;
                string kodept = command.kode_unitusaha;
                string cmt = command.komentar ?? "";

                string str_tanggalrealisasi = tanggalrealisasi.ToString("yyyy-MM-dd");

                var rmvVouDtls = await _voucherApprovalQueries.GetRemoveVoucherDetail(modifyUser, str_tanggalrealisasi, id_ms_unitusaha);
                if (rmvVouDtls.Any())
                {
                    foreach (var rmvVou in rmvVouDtls)
                    {
                        _fn_RealisasiVoucher_DetailRepository.Delete(rmvVou);
                        await _uow.CommitAsync();
                    }
                }

                var voucherBKUs = await _voucherApprovalQueries.GetVoucherBKU(modifyUser, str_tanggalrealisasi, kodept);
                if (voucherBKUs.Any())
                {
                    int idvouhdr = 0;
                    var rmvVou = await _voucherApprovalQueries.GetRemoveVoucher(modifyUser, str_tanggalrealisasi, id_ms_unitusaha);
                    if (rmvVou != null)
                    {
                        rmvVou.LastModifiedBy = modifyUser;
                        rmvVou.LastModifiedTime = now;
                        rmvVou.Komentar = cmt;
                        _fn_RealisasiVoucherRepository.Update(rmvVou);

                        await _uow.CommitAsync();

                        idvouhdr = rmvVou.ID_Fn_RealisasiVoucher;
                    }
                    else if (rmvVou == null)
                    {
                        var insertVou = new Fn_RealisasiVoucher();
                        insertVou.ID_Ms_UnitUsaha = id_ms_unitusaha;
                        insertVou.TanggalRealisasi = tanggalrealisasi;
                        insertVou.ModifyStatus = "I";
                        insertVou.LastModifiedBy = modifyUser;
                        insertVou.LastModifiedTime = now;
                        insertVou.CreatedBy = modifyUser;
                        insertVou.CreatedTime = now;
                        insertVou.StatusRelease = "N";
                        insertVou.ID_Ms_Bagian = 1;
                        insertVou.Komentar = cmt;
                        _fn_RealisasiVoucherRepository.Insert(insertVou);

                        await _uow.CommitAsync();

                        idvouhdr = insertVou.ID_Fn_RealisasiVoucher;
                    }

                    foreach (var vBKU in voucherBKUs)
                    {
                        var insertVouDtl = new Fn_RealisasiVoucher_Detail();
                        insertVouDtl.ID_Fn_RealisasiVoucher = idvouhdr;
                        insertVouDtl.NomorRekening = vBKU.keuangan;
                        insertVouDtl.NomorBKU = vBKU.bku;
                        insertVouDtl.NomorGiro = vBKU.giro;
                        insertVouDtl.NomorVoucher = vBKU.voucher;
                        insertVouDtl.Nominal = vBKU.nominal;
                        insertVouDtl.Kepada = vBKU.supplier;
                        insertVouDtl.Catatan = vBKU.keterangan;
                        insertVouDtl.FlagInclude = "I";
                        insertVouDtl.LastModifiedTime = now;
                        insertVouDtl.LastModifiedBy = modifyUser;
                        _fn_RealisasiVoucher_DetailRepository.Insert(insertVouDtl);

                        await _uow.CommitAsync();

                    }
                }

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
