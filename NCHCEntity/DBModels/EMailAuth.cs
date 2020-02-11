using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class EMailAuth
    {  
        public int StudentID { get; set; }
        public int? AuthCode { get; set; }
        public DateTime? AuthDatetime { get; set; }
        public bool HasAuth { get; set; }
        
    }
}
