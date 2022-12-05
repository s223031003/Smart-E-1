using System.ComponentModel.DataAnnotations;

namespace Smart_E.Models.Profile
{
    public class ChangePasswordPostModel
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
