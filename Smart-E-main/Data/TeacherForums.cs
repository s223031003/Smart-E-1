namespace Smart_E.Data
{
    public class TeacherForums
    {
        public Guid Id
        {
            get; set;
        }
        public string TeacherId { get; set; }

        public string ParentId { get; set; }

        public string Message { get; set; }

        public DateTime Date { get; set; }

        public bool ParentReadStatus { get; set; }

        public bool TeacherSentStatus { get; set; }


    }
}
