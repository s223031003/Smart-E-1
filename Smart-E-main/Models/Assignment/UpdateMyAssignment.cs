namespace Smart_E.Models.Assignment
{
    public class UpdateMyAssignment
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CourseId { get; set; }

        public float Mark { get; set; }       
        public float Weight { get; set; }
        public byte[] attachment { get; set; }

    }
}
