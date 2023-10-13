using BestAgroCore.Common.Domain;
using BestAgroCore.Infrastructure.Data.EFRepositories.Contracts;
using Finance.Domain.Aggregate.ApprovalSPD;
using Finance.Domain.DTO.SPD;
using Finance.Infrastructure;
using Finance.WebAPI.Application.Queries.ApprovalSPD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.WebAPI.Application.Commands.ApprovalSPD
{
    public class CreateTerimaSPDCommandHandler : ICommandHandler<CreateTerimaSPDCommand>
    {
        private readonly IUnitOfWork<FinanceContext> _uow;
        private readonly IDt_DocStatusRepository _Dt_DocStatusRepository;
        private readonly IDt_DocDeliveryStatusRepository _Dt_DocDeliveryStatusRepository;
        private readonly IDt_DocProcessStatusRepository _Dt_DocProcessStatusRepository;
        private readonly IDt_DocFlowSettingRepository _Dt_DocFlowSettingRepository;
        private readonly IApprovalSPDQueries _approvalSPDQueries;

        public CreateTerimaSPDCommandHandler(IUnitOfWork<FinanceContext> uow,
            IDt_DocStatusRepository Dt_DocStatusRepository,
            IDt_DocDeliveryStatusRepository Dt_DocDeliveryStatusRepository,
            IDt_DocProcessStatusRepository Dt_DocProcessStatusRepository,
            IDt_DocFlowSettingRepository Dt_DocFlowSettingRepository,
            IApprovalSPDQueries approvalSPDQueries)
        {
            _uow = uow;
            _Dt_DocStatusRepository = Dt_DocStatusRepository;
            _Dt_DocDeliveryStatusRepository = Dt_DocDeliveryStatusRepository;
            _Dt_DocProcessStatusRepository = Dt_DocProcessStatusRepository;
            _Dt_DocFlowSettingRepository = Dt_DocFlowSettingRepository;
            _approvalSPDQueries = approvalSPDQueries;
        }


        public async Task Handle(CreateTerimaSPDCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var listDokDirTemp = new List<DokumenSPD>(); 
                //List<Dt_DocFlowSetting> listFlowSetting = _context.Dt_DocFlowSettingSet.ToList();
                foreach (var item in command.dokumenKirimTerima)
                {
                    var qDoc = await _approvalSPDQueries.GetDocStatus(item.ID);

                    if (qDoc.LastModifiedTime != item.LastModifiedTime) // jika item sudah diedit proses lain, skip
                    {
                        continue;
                    }

                    if (item.FlowType != qDoc.FlowType && item.Status == 2) // khusus utk yg setelah penerima pertama
                    {
                        // mengalihkan
                        qDoc.FlowType = item.FlowType;
                    }
                    else
                    {
                        // menerima
                        qDoc.LastReceivedBy = command.id_ms_login;
                        qDoc.LastReceivedTime = DateTime.Now;
                        

                        // menerima sekaligus mengalihkan (utk penerima pertama)
                        if (qDoc.Status == 1)
                        {
                            qDoc.FlowType = item.FlowType;
                        }


                        //  insert DocProcessStatus, isi receivetime
                        Dt_DocProcessStatus dps = CreateNewProcessStatus(qDoc, command.id_ms_login, DateTime.Now);
                        _Dt_DocProcessStatusRepository.Insert(dps);


                        //  update DocDeliveryStatus, isi receivetime
                        var qDocDelivery = _approvalSPDQueries.getDocDeliveryStatus(qDoc.Nomor, qDoc.Jenis);
                        if (qDocDelivery != null && qDocDelivery.ID > 0)
                        {
                            qDocDelivery.ReceivedTime = qDoc.LastReceivedTime;
                            qDocDelivery.ReceivedBy = qDoc.LastReceivedBy;
                            qDocDelivery.LastModifiedBy = command.id_ms_login;
                            qDocDelivery.LastModifiedTime = DateTime.Now;
                        }
                        _Dt_DocDeliveryStatusRepository.Update(qDocDelivery);


                    }

                    qDoc.LastModifiedBy = command.id_ms_login;
                    qDoc.LastModifiedTime = DateTime.Now;

                    #region Direktur Notes
                    var infoKaryawan = await _approvalSPDQueries.GetInfoKaryawan(command.id_ms_login);
                    if (infoKaryawan.Divisi == "DIR")
                    {
                        qDoc.DirNotes = item.DirNotes;
                    }
                    #endregion

                    _Dt_DocStatusRepository.Update(qDoc);
                }

                await _uow.CommitAsync();

                AutoApprvDir(listDokDirTemp);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void AutoApprvDir(List<DokumenSPD> listDok)
        {
            var LastModifiedBy = 1;
            var GetRate = _approvalSPDQueries.getMsMataUang();

            foreach (var item in listDok)
            {
                var qSpd = _approvalSPDQueries.getFnSPD(item.RefId);

                if (qSpd != null)
                {
                    bool isValidAuto = false;
                    decimal plafonAutoApprv = 0;

                    // converts nilai permintaan menjadi IDR
                    var qRate = GetRate.Where(x => x.ID_Ms_MataUang == qSpd.ID_Ms_MataUang).FirstOrDefault();

                    var qPermintaanSpd = qSpd.Nilai * qRate.Rate;

                    if (item.Bagian == "EST" || item.Bagian == "FAC") //plafon kebun
                    {
                        plafonAutoApprv = (decimal)5000000.00; // < IDR 5 juta auto apprv
                    }

                    if (item.Bagian == "JKT" || item.Bagian == "SBY") //plafon jakarta
                    {
                        plafonAutoApprv = (decimal)5000001.00; // <= IDR 5 juta auto apprv
                    }

                    if (qSpd.ID_Ms_SPD_Kategori == 54) //* biaya entertain
                    {
                        plafonAutoApprv = (decimal)2000000.00; // < IDR 2 juta auto apprv
                    }

                    if (qPermintaanSpd < plafonAutoApprv)
                    {
                        isValidAuto = true;
                    }

                    if (isValidAuto)
                    {
                        var qDocDir = _approvalSPDQueries.GetDocStatusbyID(item.ID);

                        // menerima
                        qDocDir.LastReceivedBy = LastModifiedBy;
                        qDocDir.LastReceivedTime = DateTime.Now;
                        qDocDir.LastModifiedBy = LastModifiedBy;
                        qDocDir.LastModifiedTime = DateTime.Now;

                        _Dt_DocStatusRepository.Update(qDocDir);



                        DateTime webReqTime = DateTime.Now;
                        //  insert DocProcessStatus, isi receivetime
                        Dt_DocProcessStatus dps = CreateNewProcessStatus(qDocDir, LastModifiedBy, webReqTime);
                        _Dt_DocProcessStatusRepository.Insert(dps);



                        //  update DocDeliveryStatus, isi receivetime
                        var qDocDelivery = _approvalSPDQueries.getDocDeliveryStatus(qDocDir.Nomor, qDocDir.Jenis);
                        if (qDocDelivery != null && qDocDelivery.ID > 0)
                        {
                            qDocDelivery.ReceivedTime = qDocDir.LastReceivedTime;
                            qDocDelivery.ReceivedBy = qDocDir.LastReceivedBy;
                            qDocDelivery.LastModifiedBy = LastModifiedBy;
                            qDocDelivery.LastModifiedTime = DateTime.Now;
                        }
                        _Dt_DocDeliveryStatusRepository.Update(qDocDelivery);


                    }
                }
            }

            _uow.CommitAsync();
        }

        private Dt_DocProcessStatus CreateNewProcessStatus(Dt_DocStatus doc, int modifiedBy, DateTime webRequestTime)
        {
            Dt_DocProcessStatus newItem = new Dt_DocProcessStatus();
            newItem.Jenis = doc.Jenis;
            newItem.Nomor = doc.Nomor;
            newItem.FlowType = doc.FlowType;
            newItem.StatusLevel = doc.Status;
            newItem.ReceivedTime = (DateTime)doc.LastReceivedTime;
            newItem.ReceivedBy = (int)doc.LastReceivedBy;

            if (modifiedBy == 1)
            {
                newItem.ReceivedByDivisi = "DIR";
            }
            else
            {
                var qDivisi = _approvalSPDQueries.GetDivisi(newItem.ReceivedBy);
                newItem.ReceivedByDivisi = qDivisi;
            }

            newItem.Action = string.Empty;
            newItem.Notes = string.Empty;
            newItem.CreatedBy = modifiedBy;
            newItem.CreatedTime = DateTime.Now;
            newItem.LastModifiedBy = modifiedBy;
            newItem.LastModifiedTime = DateTime.Now;
            newItem.WebRequestTime = webRequestTime;

            return newItem;
        }
    }
}
