namespace Models.Entities
{
    public class ReportOfDelivary
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public string County { get; set; }
        public int DistancePassed { get; set; }
        public DateOnly ReportDate { get; set; }
        public TimeOnly BeginTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public TimeOnly WorkingTime { get; set; }
    }
}
