namespace Lab6.Entities;

public class Announcement
{
    public Guid? Id { get; set; }
    
    public User? User { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public int? RoomCount { get; set; }
    public DateTime? PostDate { get; set; }

    public List<Facility>? Facilities { get; set; }
    public PlacementType? PlacementType { get; set; }
    public int? PricePerDay { get; set; }

    public List<Review>? Reviews { get; set; }
}