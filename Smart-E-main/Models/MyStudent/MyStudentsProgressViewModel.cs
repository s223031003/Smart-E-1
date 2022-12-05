namespace Smart_E.Models.MyStudent
{
    public class MyStudentsProgressViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }

        public string Grade { get; set; }

        public string TeacherId { get; set; }
        public string TeacherName { get; set; }

        public string TeacherEmail { get; set; }

        public string StudentEmail { get; set; }

        public int NumberOfClasses { get; set; }
        public int NumberOfClassesAttended { get; set; }
        public int NumberOfClassesNotAttended { get; set; }

        public float YearMark { get; set; }
        public float WeightTotal { get; set; }

        public float AttendancePercentage { get; set; }



    }
}
