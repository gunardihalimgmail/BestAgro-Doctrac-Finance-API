using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Finance.Domain.DTO.SPD
{
    public class Ms_MataUang
    {
		[Key]
		public int ID_Ms_MataUang { get; set; }
		public string Nama { get; set; }
		public string Simbol { get; set; }
		public decimal Rate { get; set; }
		public DateTime TanggalAwal { get; set; }
		public DateTime TanggalAkhir { get; set; }
		public string ModifyStatus { get; set; }
	}
}
