namespace Smart_E.Models.Assignment
{
    public class CreateAssignmentPostModal
    {
        public string Name { get; set; }
        public Guid Course { get; set; }

        public float Mark { get; set; }
        public float Weight { get; set; }

        public DateTime ExpireDate { get; set; }


    }
}
