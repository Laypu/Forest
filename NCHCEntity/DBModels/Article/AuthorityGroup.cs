using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class AuthorityGroup
    {
        [Key]
        [IsSequence]
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int? Seq { get; set; }
        public bool Enable { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
    }
}
