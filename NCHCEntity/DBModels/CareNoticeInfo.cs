using SQLModel.Attributes;
using System;

namespace SQLModel.Models
{
    public class CareNoticeInfo
    {
        [Key]
        [IsSequence]
        public int ID { get; set; }
        public string NoticeNo { get; set; }
        public string ParentID { get; set; }
        public string Deleteday { get; set; }
        public string CareDesc { get; set; }
        public string CareType { get; set; }
        public string CreateDatetime { get; set; }
        public string PhotoName1 { get; set; }
        public string PhotoName2 { get; set; }
        public string PhotoName3 { get; set; }
        public string PhotoName4 { get; set; }
        public string PhotoPath1 { get; set; }
        public string PhotoPath2 { get; set; }
        public string PhotoPath3 { get; set; }
        public string PhotoPath4 { get; set; }
        public bool? isDel { get; set; }
        public string Deldatetime { get; set; }
        public string IssueDate { get; set; }
        public string VideoPath { get; set; }
        public string Name { get; set; }
        public string IDNo { get; set; }
        public string BDate { get; set; }
        public string Department { get; set; }
        public string Gender { get; set; }
        public string BedNo { get; set; }
        public string SNo { get; set; }
        public string HDate { get; set; }
        public string VideoName { get; set; }
    }
}
