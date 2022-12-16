using System.ComponentModel.DataAnnotations;
using Lab6.Entities;

namespace Lab6.Models.Announcements;

public class AnnoucementItemModel
{
    public Announcement? Announcement { get; set; }

    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Comment")]
    public string Comment { get; set; }
    
    [Required]
    [DataType(DataType.Currency)]
    [Display(Name = "Rating")]
    public int Rating { get; set; }
}