using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class MemberIndustry
    {
        public int? CompanyId { get; set; }
        public int? MemberId { get; set; }
        public int? IndustryId { get; set; }
        public int? IndustryItemId { get; set; }
    }
}
