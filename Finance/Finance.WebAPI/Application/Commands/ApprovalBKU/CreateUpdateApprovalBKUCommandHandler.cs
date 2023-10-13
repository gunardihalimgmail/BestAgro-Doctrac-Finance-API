using BestAgroCore.Common.Domain;
using BestAgroCore.Infrastructure.Data.EFRepositories.Contracts;
using Finance.Domain.Aggregate.ApprovalBkuBtu;
using Finance.Infrastructure;
using Finance.WebAPI.Application.Queries.ApprovalBKU;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.WebAPI.Application.Commands.ApprovalBKU
{
    public class CreateUpdateApprovalBKUCommandHandler : ICommandHandler<CreateUpdateApprovalBKUCommand>
    {
        private readonly IUnitOfWork<FinanceContext> _uow;
        private readonly IDt_ApprovalBkuBtuRepository _Dt_ApprovalBkuBtuRepository;
        private readonly IApprovalBKUQueries _approvalBKUQueries;


        public CreateUpdateApprovalBKUCommandHandler(IUnitOfWork<FinanceContext> uow,
            IDt_ApprovalBkuBtuRepository Dt_ApprovalBkuBtuRepository,
            IApprovalBKUQueries ApprovalBKUQueries)
        {
            _uow = uow;
            _Dt_ApprovalBkuBtuRepository = Dt_ApprovalBkuBtuRepository;
            _approvalBKUQueries = ApprovalBKUQueries;
        }


        public async Task Handle(CreateUpdateApprovalBKUCommand command, CancellationToken cancellationToken)
        {
            try
            {
                int jenisApprove = 0;

                var getDivisi = await _approvalBKUQueries.getDivisiAndJabatan(command.ID_Ms_Login);

                if (getDivisi.Divisi == "FIN")
                {
                    if (getDivisi.Jabatan == "SUPERVISOR")
                    {
                        jenisApprove = 1;
                    }
                    else if (getDivisi.Jabatan == "MANAGER")
                    {
                        jenisApprove = 2;
                    }
                }
                else if (getDivisi.Divisi == "BNC")
                {
                    jenisApprove = 3;
                }
                else if (getDivisi.Divisi == "DIR" || getDivisi.Divisi == "CEO")
                {

                }
                else
                {
                    var qGetPengganti = await _approvalBKUQueries.getMsAutentikasi(command.ID_Ms_Login);

                    if (qGetPengganti != null)
                    {
                        jenisApprove = qGetPengganti.Account == "Finance" ? 2 : qGetPengganti.Account == "BNC" ? 3 : 0;
                    }
                }


                if (command.ApprovalFlag == "Approve" || command.ApprovalFlag == "ApprovalSPVDirCEO") // Approve
                {

                    if (getDivisi.Divisi == "DIR")
                    {
                        foreach (var item in command.docKey)
                        {
                            var fullFormatNomor = ConvertToNomor(item);

                            if (fullFormatNomor.Contains("BKU") || fullFormatNomor.Contains("KKU"))
                            {
                                var idBku = await _approvalBKUQueries.getIDBKU(fullFormatNomor);

                                var qAprBku = await _approvalBKUQueries.approvalBKU(idBku.ID_Fn_BKU);

                                if (qAprBku != null)
                                {
                                    if (qAprBku.IDLogin4 == null)
                                    {
                                        qAprBku.TanggalAppr4 = DateTime.Now;
                                        qAprBku.StatusAppr4 = "Approve";
                                        qAprBku.IDLogin4 = command.ID_Ms_Login;
                                    }
                                    else
                                    {
                                        qAprBku.TanggalAppr5 = DateTime.Now;
                                        qAprBku.StatusAppr5 = "Approve";
                                        qAprBku.IDLogin5 = command.ID_Ms_Login;
                                    }

                                    _Dt_ApprovalBkuBtuRepository.Update(qAprBku);
                                    await _uow.CommitAsync();
                                }
                            }
                        }
                    }
                    else if (getDivisi.Divisi == "CEO")
                    {
                        foreach (var item in command.docKey)
                        {
                            var fullFormatNomor = ConvertToNomor(item);

                            if (fullFormatNomor.Contains("BKU") || fullFormatNomor.Contains("KKU"))
                            {
                                var idBku = await _approvalBKUQueries.getIDBKU(fullFormatNomor);

                                var qAprBku = await _approvalBKUQueries.approvalBKU(idBku.ID_Fn_BKU);


                                if (qAprBku != null)
                                {
                                    if (qAprBku.IDLogin6 == null)
                                    {
                                        qAprBku.TanggalAppr6 = DateTime.Now;
                                        qAprBku.StatusAppr6 = "Approve";
                                        qAprBku.IDLogin6 = command.ID_Ms_Login;
                                    }

                                    _Dt_ApprovalBkuBtuRepository.Update(qAprBku);
                                    await _uow.CommitAsync();
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in command.docKey)
                        {

                            var fullFormatNomor = ConvertToNomor(item);


                            if (fullFormatNomor.Contains("BKU") || fullFormatNomor.Contains("KKU"))
                            {
                                var idBku = await _approvalBKUQueries.getIDBKU(fullFormatNomor);

                                var qAprBku = await _approvalBKUQueries.approvalBKU(idBku.ID_Fn_BKU);


                                if (qAprBku != null)
                                {
                                    if ((jenisApprove == 2 && qAprBku.TanggalAppr2 != null) ||
                                        (jenisApprove == 3 && qAprBku.TanggalAppr3 != null) ||
                                        qAprBku.StatusAppr2 == "Reject" || qAprBku.StatusAppr3 == "Reject")
                                    {
                                        HandleError("false");
                                    }

                                    if (jenisApprove == 1)
                                    {
                                        qAprBku.TanggalAppr1 = DateTime.Now;
                                        qAprBku.StatusAppr1 = "Approve";
                                        qAprBku.IDLogin1 = command.ID_Ms_Login;
                                    }
                                    else if (jenisApprove == 2)
                                    {
                                        qAprBku.TanggalAppr2 = DateTime.Now;
                                        qAprBku.StatusAppr2 = "Approve";
                                        qAprBku.IDLogin2 = command.ID_Ms_Login;
                                    }
                                    else if (jenisApprove == 3)
                                    {
                                        qAprBku.TanggalAppr3 = DateTime.Now;
                                        qAprBku.StatusAppr3 = "Approve";
                                        qAprBku.IDLogin3 = command.ID_Ms_Login;
                                    }
                                    qAprBku.LastModifiedTime = DateTime.Now;
                                    qAprBku.LastModifiedBy = command.ID_Ms_Login;

                                    _Dt_ApprovalBkuBtuRepository.Update(qAprBku);
                                    await _uow.CommitAsync();
                                }
                                else
                                {
                                    Dt_ApprovalBkuBtu single = new Dt_ApprovalBkuBtu();
                                    single.Form = 9;
                                    single.FormId = idBku.ID_Fn_BKU;
                                    single.TanggalAppr1 = DateTime.Now;
                                    single.StatusAppr1 = "Approve";
                                    single.RowStatus = "I";
                                    single.LastModifiedBy = command.ID_Ms_Login;
                                    single.LastModifiedTime = DateTime.Now;
                                    single.IDLogin1 = command.ID_Ms_Login;
                                    single.IDLogin2 = 0;
                                    single.IDLogin3 = 0;

                                    _Dt_ApprovalBkuBtuRepository.Insert(single);
                                    await _uow.CommitAsync();
                                }
                            }
                            else if (fullFormatNomor.Contains("BTU") || fullFormatNomor.Contains("BMT") || fullFormatNomor.Contains("KMT"))
                            {
                                var idBtu = await _approvalBKUQueries.getIDBTU(fullFormatNomor);

                                var qAprbtu = await _approvalBKUQueries.approvalBTU(idBtu.ID_Fn_BTU);

                                if (qAprbtu != null)
                                {
                                    if ((jenisApprove == 2 && qAprbtu.TanggalAppr2 != null) ||
                                        (jenisApprove == 3 && qAprbtu.TanggalAppr3 != null) ||
                                        qAprbtu.StatusAppr2 == "Reject" || qAprbtu.StatusAppr3 == "Reject")
                                    {
                                        HandleError("false");
                                    }

                                    if (jenisApprove == 1)
                                    {
                                        qAprbtu.TanggalAppr1 = DateTime.Now;
                                        qAprbtu.StatusAppr1 = "Approve";
                                        qAprbtu.IDLogin1 = command.ID_Ms_Login;
                                    }
                                    else if (jenisApprove == 2)
                                    {
                                        qAprbtu.TanggalAppr2 = DateTime.Now;
                                        qAprbtu.StatusAppr2 = "Approve";
                                        qAprbtu.IDLogin2 = command.ID_Ms_Login;
                                    }
                                    else if (jenisApprove == 3)
                                    {
                                        qAprbtu.TanggalAppr3 = DateTime.Now;
                                        qAprbtu.StatusAppr3 = "Approve";
                                        qAprbtu.IDLogin3 = command.ID_Ms_Login;
                                    }

                                    qAprbtu.LastModifiedTime = DateTime.Now;
                                    qAprbtu.LastModifiedBy = command.ID_Ms_Login;

                                    _Dt_ApprovalBkuBtuRepository.Update(qAprbtu);
                                    await _uow.CommitAsync();
                                }
                                else
                                {
                                    Dt_ApprovalBkuBtu single = new Dt_ApprovalBkuBtu();
                                    single.Form = 10;
                                    single.FormId = idBtu.ID_Fn_BTU;
                                    single.TanggalAppr1 = DateTime.Now;
                                    single.StatusAppr1 = "Approve";
                                    single.RowStatus = "I";
                                    single.LastModifiedBy = command.ID_Ms_Login;
                                    single.LastModifiedTime = DateTime.Now;
                                    single.IDLogin1 = command.ID_Ms_Login;
                                    single.IDLogin2 = 0;
                                    single.IDLogin3 = 0;

                                    _Dt_ApprovalBkuBtuRepository.Insert(single);
                                    await _uow.CommitAsync();
                                }
                            }
                        }

                        //await _uow.CommitAsync();
                    }


                }
                else // Reject
                {
                    foreach (var item in command.docKey)
                    {
                        var fullFormatNomor = ConvertToNomor(item);

                        if (fullFormatNomor.Contains("BKU") || fullFormatNomor.Contains("KKU"))
                        {
                            var idBku = await _approvalBKUQueries.getIDBKU(fullFormatNomor);

                            var qAprBku = await _approvalBKUQueries.approvalBKU(idBku.ID_Fn_BKU);


                            if (qAprBku != null)
                            {
                                if (jenisApprove == 2)
                                {
                                    qAprBku.TanggalAppr2 = DateTime.Now;
                                    qAprBku.StatusAppr2 = "Reject";
                                    qAprBku.IDLogin2 = command.ID_Ms_Login;
                                    qAprBku.Comment2 = command.RejectionComment;
                                }
                                else if (jenisApprove == 3)
                                {
                                    qAprBku.TanggalAppr3 = DateTime.Now;
                                    qAprBku.StatusAppr3 = "Reject";
                                    qAprBku.IDLogin3 = command.ID_Ms_Login;
                                    qAprBku.Comment3 = command.RejectionComment;
                                }

                                _Dt_ApprovalBkuBtuRepository.Update(qAprBku);
                                await _uow.CommitAsync();
                            }
                        }
                        else if (fullFormatNomor.Contains("BTU") || fullFormatNomor.Contains("BMT") || fullFormatNomor.Contains("KMT"))
                        {

                            var idBtu = await _approvalBKUQueries.getIDBTU(fullFormatNomor);

                            var qAprbtu = await _approvalBKUQueries.approvalBTU(idBtu.ID_Fn_BTU);


                            if (qAprbtu != null)
                            {
                                if (jenisApprove == 2)
                                {
                                    qAprbtu.TanggalAppr2 = DateTime.Now;
                                    qAprbtu.StatusAppr2 = "Reject";
                                    qAprbtu.IDLogin2 = command.ID_Ms_Login;
                                    qAprbtu.Comment2 = command.RejectionComment;
                                }
                                else if (jenisApprove == 3)
                                {
                                    qAprbtu.TanggalAppr3 = DateTime.Now;
                                    qAprbtu.StatusAppr3 = "Reject";
                                    qAprbtu.IDLogin3 = command.ID_Ms_Login;
                                    qAprbtu.Comment3 = command.RejectionComment;
                                }

                                _Dt_ApprovalBkuBtuRepository.Update(qAprbtu);
                                await _uow.CommitAsync();
                            }
                        }
                    }

                    //await _uow.CommitAsync();
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
