using System.ComponentModel.DataAnnotations;

namespace Smart_E.Models
{
    public class Subject
    {
        [Key]
        [Required]
        public int SubjId { get; set; }
        [Required]
        public string SubjectName { get; set; }
    }
}
