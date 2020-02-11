using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class CouponList
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public int CouponPrice { get; set; }
        public string Coupon { get; set; }
        public string TaxID { get; set; }
        public bool Used { get; set; }
        public int Year { get; set; }
        public DateTime? LimitDateTime { get; set; }
        public int PaymentID { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public string UseAccount { get; set; }
        public int? MemberID { get; set; }
        public int? CompanyID { get; set; }
        public string UseName { get; set; }
        public string Note { get; set; }
        public string Code { get; set; }
        public DateTime? UseDateTime { get; set; }
        public int Type { get; set; }
        public string CompanyName { get; set; }
        public string CreateAccount { get; set; }
        public string CreateName { get; set; }
    }
}
