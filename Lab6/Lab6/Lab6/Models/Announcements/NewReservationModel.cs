namespace Lab6.Models.Announcements;

public class NewReservationModel
{
    public Guid? AnnouncementId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? TillDate { get; set; }
}