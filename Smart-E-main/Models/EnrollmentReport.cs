using System.ComponentModel.DataAnnotations;

namespace Smart_E.Models
{
    public class EnrollmentReport
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public List<Grade> Grades { get; set; }
        [Required]
        public int TotalTeachers { get; set; }
        [Required]
        public int TotalStudents { get; set; }
    }
}
