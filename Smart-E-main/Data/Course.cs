namespace Smart_E.Data
{
    public class Course
    {
        public Guid Id { get; set; }
        public string CourseName { get; set; }
        public string TeacherId { get; set; }
        public string Grade { get; set; }

        public int NumberOfClasses { get; set; }
    }
}
