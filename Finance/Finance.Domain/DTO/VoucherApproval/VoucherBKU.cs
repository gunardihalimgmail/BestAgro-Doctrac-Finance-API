using System;


namespace Finance.Domain.DTO.VoucherApproval
{
	public class VoucherBKU
	{
		public string ID { get; set; }
		public int ID_Ms_UnitUsaha { get; set; }
		public int ID_Ms_Bagian { get; set; }
		public int ID_Ms_Keuangan { get; set; }
		public string UnitUsaha { get; set; }
		public string Bagian { get; set; }
		public string KodeKeuangan { get; set; }
		public string NamaKeuangan { get; set; }
		public string Keuangan { get; set; }
		public string MataUang { get; set; }
		public DateTime Tanggal { get; set; }
		public string BKU { get; set; }
		public string Giro { get; set; }
		public string Voucher { get; set; }
		public string Nominal { get; set; }
		public string Supplier { get; set; }
		public string AtasNama { get; set; }
		public string Keterangan { get; set; }
		public string BaseOn { get; set; }

	}
}
