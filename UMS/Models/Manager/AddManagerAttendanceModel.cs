namespace UMS.Models.Manager;

public class AddManagerAttendanceModel
{
    public int ManagerId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }
    public bool IsPresent { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}