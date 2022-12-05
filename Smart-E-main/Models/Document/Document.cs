using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Smart_E.Models.Courses;

namespace Smart_E.Models.Document
{
    public class Document
    {
        [Key]
        public int FileID { get; set; }

        public string FileName { set; get; }

        public byte[] attachment { set; get; }

        [ForeignKey("Chapter")]
        public Guid ChapterID { set; get; }
        public virtual Chapter Chapter { set; get; }
    }
}
