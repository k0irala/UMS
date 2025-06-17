namespace UMS.Models.Entities
{
    public class ManagerAttendance
    {
        public int Id { get; set; }
        public string ManagerId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public bool IsPresent { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Manager Manager { get; set; }
    }
}
