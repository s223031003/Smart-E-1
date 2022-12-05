using System.ComponentModel.DataAnnotations;

namespace Smart_E.Models
{
    public class Grade
    {
        [Key]
        [Required]
        public int GradeID { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
