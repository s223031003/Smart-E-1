using System.ComponentModel.DataAnnotations;

namespace Smart_E.Models.Teachers
{
    public class EmailTeacherPostModel
    {
        [Required, Display(Name = "Your name")]
        public string FromName { get; set; }
        [Required, Display(Name = "Your email"), EmailAddress]
        public string FromEmail { get; set; }
        [Required]
        public string Message { get; set; }
        //public HttpPostedFileBase Upload { get; set; }
    }
}
