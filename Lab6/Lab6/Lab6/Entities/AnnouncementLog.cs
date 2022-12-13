namespace Lab6.Entities;

public class AnnouncementLog
{
    public Guid Id { get; set; }
    public string? Event { get; set; }
    
    public Guid? UserId { get; set; }
    public Guid? AnnouncementId { get; set; }
}