using System.ComponentModel.DataAnnotations;

namespace Smart_E.Models.Invites
{
    public class InvitesViewModal
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        public DateTime Date { get; set; }
    }
}
