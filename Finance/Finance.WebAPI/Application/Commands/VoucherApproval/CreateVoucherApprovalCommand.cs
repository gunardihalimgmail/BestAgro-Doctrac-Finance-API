using System;
using BestAgroCore.Common.Domain;

namespace Finance.WebAPI.Application.Commands.VoucherApproval
{
	public class CreateVoucherApprovalCommand : ICommand
	{
		public int id_fn_realisasivoucher { get; set; }
		public int id_ms_unitusaha { get; set; }
		public string kode_unitusaha { get; set; }
		public DateTime tanggalrealisasi { get; set; }
		public string modifystatus { get; set; }
		public int lastmodifiedby { get; set; }
		public DateTime lastmodifiedtime { get; set; }
		public int createdby { get; set; }
		public DateTime createdtime { get; set; }
		public string statusrelease { get; set; }
		public int id_ms_bagian { get; set; }
		public string komentar { get; set; }
	}
}
