using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Domain.DTO.SPD
{
    public class NoteList
    {
        public int ID_Dt_Notes { get; set; }
        public string Form { get; set; }
        public string Flag { get; set; }
        public string Referensi { get; set; }
        public string Notes { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }

        public string Username { get; set; }
    }
}
