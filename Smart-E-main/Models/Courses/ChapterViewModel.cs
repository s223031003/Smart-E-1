using Smart_E.Data;
using Smart_E.Models.NewFolder;

namespace Smart_E.Models.Courses
{
    public class ChapterViewModel
    {
        public Guid ChapterID { get; set; }

        public string ChapterName { get; set; }

        public DateTime Date { get; set; }

        public string? Description { get; set; }
        
        public List<Chapter> chapters { get; set; }

        public Guid Id { get; set; }
        public  string Course { get; set; }

        public string Grade { get; set; }

        //Dimpho
        
        public string FileName { set; get; }

        public IFormFile attachment { set; get; }


        public virtual Chapter Chapter { set; get; }        
        public List<Document.Document> documents { get; set; }


    }
}
