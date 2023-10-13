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
    public class CreateKirimSPDESTCommandHandler : ICommandHandler<CreateKirimSPDESTCommand>
    {
        private readonly IUnitOfWork<FinanceContext> _uow;
        private readonly IDt_DocStatusRepository _Dt_DocStatusRepository;
        private readonly IDt_DocDeliveryStatusRepository _Dt_DocDeliveryStatusRepository;
        private readonly IDt_DocProcessStatusRepository _Dt_DocProcessStatusRepository;
        private readonly IDt_DocFlowSettingRepository _Dt_DocFlowSettingRepository;
        private readonly IApprovalSPDQueries _approvalSPDQueries;


        public CreateKirimSPDESTCommandHandler(IUnitOfWork<FinanceContext> uow,
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

        public async Task Handle(CreateKirimSPDESTCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var listDokDirTemp = new List<DokumenSPD>();

                foreach (var item in command.dokumenKirimTerima)
                {
                    var qDoc = await _approvalSPDQueries.GetDocStatusKirim(item.ID, item.Nomor);

                    if (qDoc != null && qDoc.LastModifiedTime != item.LastModifiedTime)     // jika item sudah diedit proses lain, skip
                    {
                        continue;
                    }

                    if (qDoc == null || qDoc.ID == 0 || item.Status == 0)
                    {
                        // kiriman requester. insert new document
                        Dt_DocStatus newdok = new Dt_DocStatus();
                        newdok.Bagian = item.Bagian;
                        newdok.FlowType = item.FlowType;
                        newdok.Jenis = item.Jenis;
                        newdok.LastModifiedBy = command.id_ms_login;
                        newdok.LastModifiedTime = DateTime.Now;
                        newdok.LastNotes = item.NewNotes ?? "";
                        newdok.Nomor = item.Nomor;
                        newdok.RefId = item.RefId;
                        newdok.Perihal = item.Perihal;
                        newdok.PT = item.PT;
                        newdok.RequestBy = command.id_ms_login;
                        newdok.RequestTime = DateTime.Now;
                        newdok.Status = 1;
                        newdok.StatusBy = command.id_ms_login;
                        newdok.StatusTime = DateTime.Now;
                        newdok.CreatedBy = command.id_ms_login;
                        newdok.CreatedTime = DateTime.Now;
                        newdok.RowStatus = "I";

                        _Dt_DocStatusRepository.Insert(newdok);


                        // insert DocDeliveryStatus, isi SendTime
                        Dt_DocDeliveryStatus dps = CreateNewDeliveryStatus(newdok, command.id_ms_login);
                        _Dt_DocDeliveryStatusRepository.Insert(dps);

                        await _uow.CommitAsync();

                    }
                    else if (qDoc.RowStatus != "R") // hanya bisa kirim utk yang tidak pernah diREJECT BY JVE
                    {
                        if (qDoc.Status == 1) // khusus utk penerima pertama bisa mengalihkan saat mengirim
                        {
                            qDoc.FlowType = item.FlowType;
                        }

                        // changes 22.05 untuk status yang awalnya reject kemudian dikirim ulang, langsung menuju PIC, tanpa melewati CS
                        // karena yang dikirim ulang hanya jika kurang kelengkapan saja
                        // utk yang perlu kembalikan dokumen, SPD nya harus dibuat lagi

                        qDoc.IsRejected = false; // kirim ulang dari ke PIC selanjutnya, utk dokumen yang direject
                        qDoc.Status = qDoc.Status + 1;

                        qDoc.StatusBy = command.id_ms_login;
                        qDoc.StatusTime = DateTime.Now;
                        qDoc.LastNotes = item.NewNotes ?? "";
                        qDoc.LastModifiedBy = command.id_ms_login;
                        qDoc.LastModifiedTime = DateTime.Now;

                        _Dt_DocStatusRepository.Update(qDoc);


                        // insert DocDeliveryStatus, isi SendTime
                        Dt_DocDeliveryStatus dds = CreateNewDeliveryStatus(qDoc, command.id_ms_login);
                        _Dt_DocDeliveryStatusRepository.Insert(dds);


                        // update DocProcessStatus isi send time 
                        var qDocProcess = await _approvalSPDQueries.GetDocProcessStatus(qDoc.Nomor, qDoc.Jenis);
                        if (qDocProcess != null && qDocProcess.ID > 0)
                        {
                            qDocProcess.SendTime = qDoc.StatusTime;
                            qDocProcess.SendBy = qDoc.StatusBy;
                            qDocProcess.Notes = qDoc.LastNotes;
                            qDocProcess.Action = string.Empty;
                            qDocProcess.LastModifiedBy = command.id_ms_login;
                            qDocProcess.LastModifiedTime = DateTime.Now;
                        }

                        _Dt_DocProcessStatusRepository.Update(qDocProcess);


                        await _uow.CommitAsync();

                        #region Auto Approve Dir

                        //region code untuk auto approvall DIR
                        var qDocFlowSetting = await _approvalSPDQueries.getDtDocFlowSetting(qDoc.FlowType, qDoc.Bagian, qDoc.Jenis, qDoc.Status);
                        if (qDocFlowSetting != null)
                        {
                            var nxtPic = qDocFlowSetting.Pic;
                            if (nxtPic.Contains("DIR"))
                            {
                                var LastModifiedBy = 1;
                                var GetRate = _approvalSPDQueries.getMsMataUang();

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

                                        // menerima
                                        qDoc.LastReceivedBy = LastModifiedBy;
                                        qDoc.LastReceivedTime = DateTime.Now;
                                        qDoc.LastModifiedBy = LastModifiedBy;
                                        qDoc.LastModifiedTime = DateTime.Now;

                                        _Dt_DocStatusRepository.Update(qDoc);



                                        DateTime webReqTime = DateTime.Now;
                                        //  insert DocProcessStatus, isi receivetime
                                        Dt_DocProcessStatus dps = CreateNewProcessStatus(qDoc, LastModifiedBy, webReqTime);
                                        _Dt_DocProcessStatusRepository.Insert(dps);


                                        var qDocDelivery = _approvalSPDQueries.getDocDeliveryStatus(qDoc.Nomor, qDoc.Jenis);
                                        if (qDocDelivery != null && qDocDelivery.ID > 0)
                                        {
                                            dds.ReceivedTime = qDoc.LastReceivedTime;
                                            dds.ReceivedBy = qDoc.LastReceivedBy;
                                            dds.LastModifiedBy = LastModifiedBy;
                                            dds.LastModifiedTime = DateTime.Now;
                                        }
                                        
                                        _Dt_DocDeliveryStatusRepository.Update(dds);


                                    }
                                }
                            }

                            await _uow.CommitAsync();
                        }

                        #endregion Auto Approve Dir
                    }
                }

                
                //AutoApprvDir(listDokDirTemp);

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
