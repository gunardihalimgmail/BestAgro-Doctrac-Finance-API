using BestAgroCore.Common.Domain;
using Finance.Domain.DTO.SPD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.WebAPI.Application.Commands.ApprovalSPD
{
    public class GetListDokumenCommand : ICommand
    {
        public int id_ms_login { get; set; }
        public List<string> Pt { get; set; }
        public string pt_join { get; set; }
        public string Keyword { get; set; }
        public string Startdate { get; set; }
        public string Enddate { get; set; }

    }

    public class CreateKirimSPDHOCommand : ICommand
    {
        public int id_ms_login { get; set; }
        public IEnumerable<string> dokKey { get; set; }
    }

    public class CreateKirimSPDESTCommand : ICommand
    {
        public int id_ms_login { get; set; }
        public List<DokumenSPD> dokumenKirimTerima { get; set; }
    }

    public class CreateKirimSPDCommand : ICommand
    {
        public int id_ms_login { get; set; }
        public List<DokumenSPD> dokumenKirimTerima { get; set; }
    }

    public class CreateTerimaSPDCommand : ICommand
    {
        public int id_ms_login { get; set; }
        public List<DokumenSPD> dokumenKirimTerima { get; set; }
    }
    
    public class CreateTolakSPDCommand : ICommand
    {
        public int id_ms_login { get; set; }
        public List<DokumenSPD> dokumenKirimTerima { get; set; }
    }

    public class CreateCommentCommand : ICommand
    {
        public string CommentNotes { get; set; }
        public int ID { get; set; }
        public string Nomor { get; set; }
    }

    public class UpdateDitujukanCommand : ICommand
    {
        public string ditujukanStr { get; set; }
        public int ID { get; set; }
        public string Nomor { get; set; }
    }

}
