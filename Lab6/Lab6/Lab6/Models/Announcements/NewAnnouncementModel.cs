using System.ComponentModel.DataAnnotations;
using Lab6.Entities;

namespace Lab6.Models.Announcements;

public class NewAnnouncementModel
{
    [Required]
    [Display(Name = "Title")]
    public string Title { get; set; }
    
    [Required]
    [Display(Name = "Description")]
    public string Description { get; set; }
    
    [Required]
    [Display(Name = "Address")]
    public string Address { get; set; }
    
    [Required]
    [Display(Name = "Room count")]
    public int RoomCount { get; set; }
    
    [Required]
    [Display(Name = "Placement type")]
    public Guid PlacementTypeId { get; set; }
    
    [Required]
    [Display(Name = "Price per day")]
    public int PricePerDay { get; set; }
    
    [Display(Name = "Facilities")]
    public List<Facility>? Facilities { get; set; }
}