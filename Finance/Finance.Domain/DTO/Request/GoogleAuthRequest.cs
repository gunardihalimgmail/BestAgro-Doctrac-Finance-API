using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Domain.DTO.Request
{
    public class GoogleAuthRequest
    {
        public int ID_Ms_Login { get; set; }
        public string AuthCodeFromPhone { get; set; }
    }
}
