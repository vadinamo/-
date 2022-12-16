namespace Lab6.Entities;

public class User
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? RoleName { get; set; }
    public List<Announcement>? Announcements { get; set; }
    public List<Review>? Reviews { get; set; }
}