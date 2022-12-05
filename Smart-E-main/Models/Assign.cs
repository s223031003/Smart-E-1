using System.ComponentModel.DataAnnotations;

namespace Smart_E.Models
{
    public class Assign
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public int SubjId { get; set; }
        [Required]
        public int TeacherId { get; set; }
    }
}
