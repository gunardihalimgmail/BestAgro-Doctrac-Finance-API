using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Finance.Domain.Aggregate.VoucherApproval
{
	public class Fn_RealisasiVoucher_Detail
	{
		[Key]
		public int ID_Fn_RealisasiVoucher_Detail { get; set; }
		public int ID_Fn_RealisasiVoucher { get; set; }
		public string NomorRekening { get; set; }
		public string NomorBKU { get; set; }
		public string NomorGiro { get; set; }
		public string NomorVoucher { get; set; }

		[Column(TypeName = "decimal(18,0)")]
		public decimal? Nominal { get; set; }
		public string Kepada { get; set; }
		public string Catatan { get; set; }
		public string FlagInclude { get; set; }
		public DateTime? LastModifiedTime { get; set; }
		public int LastModifiedBy { get; set; }

	}
}
