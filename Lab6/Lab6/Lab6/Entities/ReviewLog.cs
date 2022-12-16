namespace Lab6.Entities;

public class ReviewLog
{
    public Guid Id { get; set; }
    public string? Event { get; set; }
    
    public Guid? UserId { get; set; }
    public Guid? AnnouncementId { get; set; }
    public Guid? ReviewId { get; set; }
}