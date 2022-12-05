using Smart_E.Data;

namespace Smart_E.Models.Courses
{
    public class Chapter
    {
        public Guid ChapterID { get; set; }

        public string ChapterName { get; set; }

        public DateTime Date { get; set; }

        public string? Description { get; set; }

        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }
    }
}
