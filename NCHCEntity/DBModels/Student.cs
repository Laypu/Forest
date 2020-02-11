using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class Student
    {
        [Key]
        [IsSequence]
        public int? ID { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string CHNName { get; set; }
        public string ENGName { get; set; }
        public int? Gender { get; set; }
        public string EMail { get; set; }
        public bool OrderEPaper { get; set; }
        public string TaxID { get; set; }
        public string CHNCompanyName { get; set; }
        public string ENGCompanyName { get; set; }
        public string ContactTEL { get; set; }
        public string ContactFax { get; set; }
        public string Address { get; set; }
        public int? Status { get; set; }
        public bool VIP { get; set; }
        [EmptyNull]
        public DateTime? VIPSTDate { get; set; }
        [EmptyNull]
        public DateTime? VIPEDDate { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateUser { get; set; }
        public string LastLoginTime { get; set; }
    }
}
