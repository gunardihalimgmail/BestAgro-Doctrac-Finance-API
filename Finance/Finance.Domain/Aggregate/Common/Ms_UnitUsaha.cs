using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Finance.Domain.Aggregate.Common
{
	public class Ms_UnitUsaha
	{
		[Key]
		public int ID_Ms_UnitUsaha { get; set; }
		public string Kode { get; set; }
		public string Nama { get; set; }
		public string ModifyStatus { get; set; }
	}
}
