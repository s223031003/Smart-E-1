using Microsoft.AspNetCore.Identity;
using Smart_E.Data;

namespace Smart_E.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; } = "Active";
    }
}
