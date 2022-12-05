namespace Smart_E.Data
{
    public class AssignmentResults
    {
        public Guid Id { get; set; }
        public Guid AssignmentId { get; set; }

        public float NewMark { get; set; }

        public bool Outstanding { get; set; }
        public string StudentId {get;set;}
    }
}
