namespace Lab6.Entities;

public class Reservation
{
    public Guid Id { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? TillDate { get; set; }
    
    public Guid? UserId { get; set; }
    public Guid? AnnouncementId { get; set; }
}