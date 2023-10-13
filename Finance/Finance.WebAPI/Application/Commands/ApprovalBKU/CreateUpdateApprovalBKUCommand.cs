using BestAgroCore.Common.Domain;
using System.Collections.Generic;

namespace Finance.WebAPI.Application.Commands.ApprovalBKU
{
    public class CreateUpdateApprovalBKUCommand : ICommand
    {
        //public int ID_Ms_Login_Replacement { get; set; }
        public int ID_Ms_Login { get; set; }
        public string JabatanFlag { get; set; }
        public string AuthCodeFromPhone { get; set; }
        public string RejectionComment { get; set; }
        public string ApprovalFlag { get; set; }
        public IEnumerable<string> docKey { get; set; }
    }

    public class nomorDokumen
    {
        public int ID_Ms_Login { get; set; }
        public IEnumerable<string> docKey { get; set; }
    }
}
