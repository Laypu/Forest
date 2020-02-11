using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class ArticleItem
    {
        [Key]
        [IsSequence]
        public int ItemID { get; set; }
        public int? ModelID { get; set; }
        public int? GroupID { get; set; }
        public int? Sort { get; set; }
        public int? Lang_ID { get; set; }
        public string Title { get; set; }
        public int? ClickCnt { get; set; }
        public int? Link_Mode { get; set; }
        public string HtmlContent { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public string UploadFileDesc { get; set; }
        public string ImageFileName { get; set; }
        public string ImageFileOrgName { get; set; }
        public string PageImageFileName { get; set; }
        public string PageImageFileOrgName { get; set; }
        public string ImageFileDesc { get; set; }
        public string ImageFileLocation { get; set; }
        public string LinkUrl { get; set; }
        public bool? Enabled { get; set; }
        [EmptyNull]
        public DateTime? StDate { get; set; }
        [EmptyNull]
        public DateTime? EdDate { get; set; }
        public string PublicshDate { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        [EmptyNull]
        public int? ActiveID { get; set; }
        [EmptyNull]
        public int? ActiveItemID { get; set; }
        public bool? MemberView { get; set; }
        public bool GeneralStudentAuth { get; set; }
        public bool VIPAuth { get; set; }
        public bool EMailAuth { get; set; }
        public bool EnterpriceStudentAuth { get; set; }
    }
}
