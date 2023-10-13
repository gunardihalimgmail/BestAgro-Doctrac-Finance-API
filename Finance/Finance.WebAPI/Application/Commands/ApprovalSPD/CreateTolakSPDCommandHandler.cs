using BestAgroCore.Common.Domain;
using BestAgroCore.Infrastructure.Data.EFRepositories.Contracts;
using Finance.Domain.Aggregate.ApprovalSPD;
using Finance.Infrastructure;
using Finance.WebAPI.Application.Queries.ApprovalSPD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.WebAPI.Application.Commands.ApprovalSPD
{
    public class CreateTolakSPDCommandHandler : ICommandHandler<CreateTolakSPDCommand>
    {
        private readonly IUnitOfWork<FinanceContext> _uow;
        private readonly IDt_DocStatusRepository _Dt_DocStatusRepository;
        private readonly IDt_DocDeliveryStatusRepository _Dt_DocDeliveryStatusRepository;
        private readonly IDt_DocProcessStatusRepository _Dt_DocProcessStatusRepository;
        private readonly IFn_SPDRepository _Fn_SPDRepository;
        private readonly IDt_NotesRepository _Dt_NotesRepository;
        private readonly IApprovalSPDQueries _approvalSPDQueries;

        public CreateTolakSPDCommandHandler(IUnitOfWork<FinanceContext> uow,
            IDt_DocStatusRepository Dt_DocStatusRepository,
            IDt_DocDeliveryStatusRepository Dt_DocDeliveryStatusRepository,
            IDt_DocProcessStatusRepository Dt_DocProcessStatusRepository,
            IFn_SPDRepository Fn_SPDRepository, IDt_NotesRepository Dt_NotesRepository,
            IApprovalSPDQueries approvalSPDQueries)
        {
            _uow = uow;
            _Dt_DocStatusRepository = Dt_DocStatusRepository;
            _Dt_DocDeliveryStatusRepository = Dt_DocDeliveryStatusRepository;
            _Dt_DocProcessStatusRepository = Dt_DocProcessStatusRepository;
            _Fn_SPDRepository = Fn_SPDRepository;
            _Dt_NotesRepository = Dt_NotesRepository;
            _approvalSPDQueries = approvalSPDQueries;
        }


        public async Task Handle(CreateTolakSPDCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.id_ms_login == 2 || command.id_ms_login == 3 || command.id_ms_login == 4 || command.id_ms_login == 1587)
                {
                    // Tolak SPD yg di kirim dari manager
                    foreach (var item in command.dokumenKirimTerima)
                    {
                        string nomorSPD = item.Nomor.ToString().Trim();

                        var qDoc = await _approvalSPDQueries.TolakDir(item.Nomor);

                        Dt_Notes singleNotes = new Dt_Notes();

                        singleNotes.Form = "SPD";
                        singleNotes.Flag = "SPD - Reject - Direktur";
                        singleNotes.Referensi = nomorSPD;
                        singleNotes.Notes = item.DirNotes;
                        singleNotes.ModifiedDate = DateTime.Now;
                        singleNotes.ModifiedBy = command.id_ms_login;

                        _Dt_NotesRepository.Insert(singleNotes);

                        await _uow.CommitAsync();
                    }
                }
                else
                {

                    // Tolak SPD Terima
                    foreach (var item in command.dokumenKirimTerima)
                    {
                        var qDoc = await _approvalSPDQueries.GetDocStatus(item.ID);

                        // tolak SPD
                        if (qDoc.Jenis == "SPD")
                        {

                            if (qDoc.LastModifiedTime != item.LastModifiedTime)     // jika item sudah diedit proses lain, skip
                            {
                                continue;
                            }

                            if (qDoc.Bagian == "EST" || qDoc.Bagian == "FAC") // untuk tolak SPD KEBUN (kembalikan ke PIC sebelumnya), flow mundur
                            {

                                qDoc.RejectedBy = command.id_ms_login;
                                qDoc.IsRejected = true;
                                qDoc.LastNotes = item.NewNotes ?? "";
                                if (qDoc.Status > 1)
                                {
                                    qDoc.Status = qDoc.Status - 1;
                                    qDoc.StatusBy = command.id_ms_login;
                                    qDoc.StatusTime = DateTime.Now;
                                }

                                qDoc.LastModifiedBy = command.id_ms_login;
                                qDoc.LastModifiedTime = DateTime.Now;

                                _Dt_DocStatusRepository.Update(qDoc);



                                // insert DocDeliveryStatus, isi SendTime
                                Dt_DocDeliveryStatus dps = CreateNewDeliveryStatus(qDoc, command.id_ms_login);
                                _Dt_DocDeliveryStatusRepository.Insert(dps);



                                // update DocProcessStatus isi send time 
                                var qDocProcess = await _approvalSPDQueries.GetDocProcessStatus(qDoc.Nomor, qDoc.Jenis);
                                if (qDocProcess != null && qDocProcess.ID > 0)
                                {
                                    qDocProcess.SendTime = DateTime.Now;
                                    qDocProcess.SendBy = command.id_ms_login;
                                    qDocProcess.Notes = qDoc.LastNotes;
                                    qDocProcess.Action = "REJECT";
                                    qDocProcess.LastModifiedBy = command.id_ms_login;
                                    qDocProcess.LastModifiedTime = DateTime.Now;
                                }
                                _Dt_DocProcessStatusRepository.Update(qDocProcess);


                            }
                            else // untuk flow SPD HO, Reject SPD tersebut dan Hapus Doctrac nya
                            {
                                qDoc.RejectedBy = command.id_ms_login;
                                qDoc.IsRejected = true;
                                qDoc.LastNotes = item.NewNotes ?? "";
                                if (qDoc.Status > 1)
                                {
                                    qDoc.Status = qDoc.Status - 1;
                                    qDoc.StatusBy = command.id_ms_login;
                                    qDoc.StatusTime = DateTime.Now;
                                }

                                qDoc.LastModifiedBy = command.id_ms_login;
                                qDoc.LastModifiedTime = DateTime.Now;
                                qDoc.RowStatus = "D"; // hapus DocStatus SPDnya

                                _Dt_DocStatusRepository.Update(qDoc);



                                // insert DocDeliveryStatus, isi SendTime
                                Dt_DocDeliveryStatus dps = CreateNewDeliveryStatus(qDoc, command.id_ms_login);
                                _Dt_DocDeliveryStatusRepository.Insert(dps);



                                // update DocProcessStatus isi send time 
                                var qDocProcess = await _approvalSPDQueries.GetDocProcessStatus(qDoc.Nomor, qDoc.Jenis);
                                if (qDocProcess != null && qDocProcess.ID > 0)
                                {
                                    qDocProcess.SendTime = DateTime.Now;
                                    qDocProcess.SendBy = command.id_ms_login;
                                    qDocProcess.Notes = qDoc.LastNotes;
                                    qDocProcess.Action = "REJECT";
                                    qDocProcess.LastModifiedBy = command.id_ms_login;
                                    qDocProcess.LastModifiedTime = DateTime.Now;
                                }

                                _Dt_DocProcessStatusRepository.Update(qDocProcess);



                                // reject SPD nya
                                var qSpd = await _approvalSPDQueries.getFnSPDTolak(qDoc.RefId);
                                qSpd.Status = "R";
                                qSpd.KeteranganStatus = "Rejected";

                                _Fn_SPDRepository.Update(qSpd);

                            }
                        }
                    }

                    await _uow.CommitAsync();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private Dt_DocDeliveryStatus CreateNewDeliveryStatus(Dt_DocStatus doc, int modifiedBy)
        {
            Dt_DocDeliveryStatus newItem = new Dt_DocDeliveryStatus();
            newItem.Jenis = doc.Jenis;
            newItem.Nomor = doc.Nomor;
            newItem.FlowType = doc.FlowType;
            newItem.StatusLevel = doc.Status;
            newItem.SendTime = doc.StatusTime;
            newItem.SendBy = doc.StatusBy;

            var qDivisi = _approvalSPDQueries.GetDivisi(newItem.SendBy);
            newItem.SendByDivisi = qDivisi;

            newItem.LastModifiedBy = modifiedBy;
            newItem.LastModifiedTime = DateTime.Now;
            return newItem;
        }

    }
}
