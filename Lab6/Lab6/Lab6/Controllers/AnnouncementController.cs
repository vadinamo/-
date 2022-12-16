using Lab6.Entities;
using Lab6.Models.Announcements;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Lab6.Controllers;

public class AnnouncementController: Controller
{
    private readonly NpgsqlCommand _dbcommand;

    public AnnouncementController()
    {
        _dbcommand = DBManager.getCommand();
    }

    public IActionResult AllAnnouncements()
    {
        return View(GetAnnouncements());
    }

    public AllAnnouncementsModel GetAnnouncements()
    {
        _dbcommand.CommandText = 
            @"SELECT Announcements.id, Announcements.title, Announcements.description, Announcements.address, 
Announcements.room_count, Announcements.post_date, Announcements.price_per_day,
Users.id, Users.username, Placement_types.placement_type, Facilities.facility_name 
FROM Announcements LEFT JOIN Users ON Users.id = Announcements.user_id
	LEFT JOIN Placement_types ON Placement_types.id = Announcements.placement_type_id
		LEFT JOIN Announcement_has_facility ON Announcements.id = Announcement_has_facility.announcement_id
			LEFT JOIN Facilities ON Facilities.id = Announcement_has_facility.facility_id";

        var model = new AllAnnouncementsModel();
        model.Announcements = new List<Announcement>();
        
        var dataReader = _dbcommand.ExecuteReader();
        var announcement = new Announcement();
        while (dataReader.Read())
        {
            if ((Guid)dataReader.GetValue(0) != announcement.Id)
            {
                announcement = new Announcement();
                announcement.Id = (Guid)dataReader.GetValue(0);
                announcement.Title = (string)dataReader.GetValue(1);
                announcement.Description = (string)dataReader.GetValue(2);
                announcement.Address = (string)dataReader.GetValue(3);
                announcement.RoomCount = (int)dataReader.GetValue(4);
                announcement.PostDate = (DateTime)dataReader.GetValue(5);
                announcement.PricePerDay = (int)dataReader.GetValue(6);
                announcement.User = new User
                {
                    Id = (Guid)dataReader.GetValue(7),
                    Username = (string)dataReader.GetValue(8)
                };
                announcement.PlacementType = new PlacementType
                {
                    PlacementTypeName = (string)dataReader.GetValue(9)
                };
                announcement.Facilities = new List<Facility>();
                announcement.Facilities.Add(new Facility
                {
                    FacilityName = (string)dataReader.GetValue(10)
                });
                model.Announcements.Add(announcement);
            }
            else
            {
                announcement.Facilities.Add(new Facility { FacilityName = (string)dataReader.GetValue(10) });
            }
        }
        
        dataReader.Close();
        return model;
    }
    
    public IActionResult AnnouncementItem(Guid id)
    {
        
        return View(GetAnnouncement(id));
    }

    public AnnoucementItemModel GetAnnouncement(Guid id)
    {
        _dbcommand.CommandText = 
            @"SELECT Announcements.id, Announcements.title, Announcements.description, Announcements.address, 
Announcements.room_count, Announcements.post_date, Announcements.price_per_day,
Users.id, Users.username, Placement_types.placement_type, Facilities.facility_name 
FROM Announcements LEFT JOIN Users ON Users.id = Announcements.user_id
	LEFT JOIN Placement_types ON Placement_types.id = Announcements.placement_type_id
		LEFT JOIN Announcement_has_facility ON Announcements.id = Announcement_has_facility.announcement_id
			LEFT JOIN Facilities ON Facilities.id = Announcement_has_facility.facility_id WHERE Announcements.id = (@p1)";
        var params1 = _dbcommand.CreateParameter();
        
        params1.ParameterName = "p1";
        params1.Value = id;
        
        _dbcommand.Parameters.Add(params1);
        
        var dataReader = _dbcommand.ExecuteReader();
        
        var model = new AnnoucementItemModel();
        var announcement = new Announcement();
        while (dataReader.Read())
        {
            if ((Guid)dataReader.GetValue(0) != announcement.Id)
            {
                announcement.Id = (Guid)dataReader.GetValue(0);
                announcement.Title = (string)dataReader.GetValue(1);
                announcement.Description = (string)dataReader.GetValue(2);
                announcement.Address = (string)dataReader.GetValue(3);
                announcement.RoomCount = (int)dataReader.GetValue(4);
                announcement.PostDate = (DateTime)dataReader.GetValue(5);
                announcement.PricePerDay = (int)dataReader.GetValue(6);
                announcement.User = new User
                {
                    Id = (Guid)dataReader.GetValue(7),
                    Username = (string)dataReader.GetValue(8)
                };
                announcement.PlacementType = new PlacementType
                {
                    PlacementTypeName = (string)dataReader.GetValue(9)
                };
                announcement.Facilities = new List<Facility>();
                announcement.Facilities.Add(new Facility
                {
                    FacilityName = (string)dataReader.GetValue(10)
                });
            }
            else
            {
                announcement.Facilities.Add(new Facility { FacilityName = (string)dataReader.GetValue(10) });
            }
        }

        dataReader.Close();
        _dbcommand.Parameters.Clear();
        
        announcement.Reviews = GetReviews(id);
        model.Announcement = announcement;
        return model;
    }

    public List<Review> GetReviews(Guid id)
    {
        _dbcommand.CommandText = @"SELECT Reviews.id, Reviews.rating, Reviews.review, Reviews.post_date, Reviews.user_id, Users.username
FROM Reviews 
	LEFT JOIN Users on Users.id = Reviews.user_id
	WHERE announcement_id = (@p1)";
        
        var params1 = _dbcommand.CreateParameter();
        params1.ParameterName = "p1";
        params1.Value = id;
        
        _dbcommand.Parameters.Add(params1);
        
        var reviews = new List<Review>();
        var dataReader = _dbcommand.ExecuteReader();
        while (dataReader.Read())
        {
            reviews.Add(new Review
                {
                    Id = (Guid)dataReader.GetValue(0),
                    Rating = (int)dataReader.GetValue(1),
                    Comment = (string)dataReader.GetValue(2),
                    PostDate = (DateTime)dataReader.GetValue(3),
                    User = new User
                    {
                        Id = (Guid)dataReader.GetValue(4),
                        Username = (string)dataReader.GetValue(5)
                    }
                }
            );
        }
        
        dataReader.Close();
        _dbcommand.Parameters.Clear();
        return reviews;
    }

    public async Task<IActionResult> PostReview(Guid id, AnnoucementItemModel model, string returnUrl)
    {
        _dbcommand.CommandText = @"INSERT INTO Reviews VALUES (
	gen_random_uuid(),
	(@p1),
	(@p2),
	(@p3),
	(@p4),
	(SELECT id FROM Users WHERE email = (@p5)));";
        var params1 = _dbcommand.CreateParameter();
        var params2 = _dbcommand.CreateParameter();
        var params3 = _dbcommand.CreateParameter();
        var params4 = _dbcommand.CreateParameter();
        var params5 = _dbcommand.CreateParameter();
        
        params1.ParameterName = "p1";
        params1.Value = model.Rating;
        
        params2.ParameterName = "p2";
        params2.Value = model.Comment;
        
        params3.ParameterName = "p3";
        params3.Value = DateTime.Now;
        
        params4.ParameterName = "p4";
        params4.Value = id;
        
        params5.ParameterName = "p5";
        params5.Value = User.Identity.Name;
        
        _dbcommand.Parameters.Add(params1);
        _dbcommand.Parameters.Add(params2);
        _dbcommand.Parameters.Add(params3);
        _dbcommand.Parameters.Add(params4);
        _dbcommand.Parameters.Add(params5);
        
        _dbcommand.ExecuteReader();
        _dbcommand.Parameters.Clear();
        
        return Redirect(returnUrl);
    }
}