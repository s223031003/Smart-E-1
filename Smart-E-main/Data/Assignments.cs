using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Primitives;

namespace Smart_E.Data
{
    public class Assignments
    {
        public Guid Id { get; set; }

        public string Name { get; set; } 

        public Guid CourseId { get; set; }

        public float Weight { get; set; }
        public float Mark { get; set; }
        public DateTime ExpireDate { get; set; }

    }
}
