using System.ComponentModel;
using DocumentFormat.OpenXml.Presentation;

namespace Smart_E.Data
{
    public class MyCourses
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }
        public string StudentId { get; set; }

        [DefaultValue("1")]
        public int NumberOfClassesAttended  { get; set; } 

        public bool Status { get; set; }


    }
}
