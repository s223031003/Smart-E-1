using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;

namespace Smart_E.Models.Departments
{
    public class DepartmentViewModel
    {
        public Guid Id { get; set; }
        public string DepartmentName { get; set; }
        public string HODId { get; set; }
        public string HOD { get; set; }
        public Guid CourseId { get; set; }
        public string Subject { get; set; }
    }
}
