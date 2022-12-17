using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Lab6.Entities;

namespace Lab6.Models.Announcements;

public class EditReviewModel
{
    public Guid Id { get; set; }

    [DataType(DataType.Text)] 
    public string? Comment { get; set; }
    
    [DataType(DataType.Currency)] 
    public int Rating { get; set; }
}