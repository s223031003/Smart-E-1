namespace Smart_E.Data
{
    public class Calendar
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Start { get; set; }
        public string End { get; set; }

        public bool IsFullDayEvent { get; set; }
        public string Theme { get; set; }

        public string Subject { get; set; }

    }
}
