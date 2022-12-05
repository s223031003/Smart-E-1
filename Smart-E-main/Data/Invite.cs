namespace Smart_E.Data
{
    public class Invite
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }

        public string InviteTo { get; set; }

        public string InviteFrom { get; set; }

        public bool Status { get; set; }

        public string Message { get; set; }
    }
}
