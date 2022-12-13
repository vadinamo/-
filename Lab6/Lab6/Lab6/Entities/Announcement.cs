namespace Lab6.Entities;

public class Announcement
{
    public Guid Id { get; set; }
    
    public Guid? UserId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public int? RoomCount { get; set; }
    public DateTime? PostDate { get; set; }

    public Guid? PlacementTypeId { get; set; }
    public int? PricePerDay { get; set; }
}