namespace Lab6.Entities;

public class Review
{
    public Guid? Id { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime? PostDate { get; set; }
    
    public User? User { get; set; }
    public Guid? Announcement_id { get; set; }
}