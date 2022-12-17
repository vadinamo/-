using System.Data;
using Lab6.Entities;
using Lab6.Models.Announcements;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
	JOIN Placement_types ON Placement_types.id = Announcements.placement_type_id
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
                if (dataReader.GetValue(10) != DBNull.Value)
                {
                    announcement.Facilities.Add(new Facility
                    {
                        FacilityName = (string)dataReader.GetValue(10)
                    });
                }
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
FROM Announcements 
    JOIN Users ON Users.id = Announcements.user_id
	    JOIN Placement_types ON Placement_types.id = Announcements.placement_type_id
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
                if (dataReader.GetValue(10) != DBNull.Value)
                {
                    announcement.Facilities.Add(new Facility
                    {
                        FacilityName = (string)dataReader.GetValue(10)
                    });
                }
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

    [HttpGet]
    public IActionResult ReservationCreating(Guid id)
    {
        return View(new NewReservationModel
        {
            AnnouncementId = id
        });
    }

    [HttpPost]
    public async Task<IActionResult> ReservationCreating(NewReservationModel model, Guid announcementId)
    {
        if (ModelState.IsValid)
        {
            _dbcommand.CommandText = @"INSERT INTO Reservations VALUES (
	gen_random_uuid(),
	(@p1),
	(@p2),
	(SELECT id FROM Users WHERE email = (@p3)),
	(@p4)
)";
            var params1 = _dbcommand.CreateParameter();
            var params2 = _dbcommand.CreateParameter();
            var params3 = _dbcommand.CreateParameter();
            var params4 = _dbcommand.CreateParameter();
            
            params1.ParameterName = "p1";
            params1.Value = model.FromDate;
            
            params2.ParameterName = "p2";
            params2.Value = model.TillDate;
            
            params3.ParameterName = "p3";
            params3.Value = User.Identity.Name;
            
            params4.ParameterName = "p4";
            params4.Value = announcementId;
        
            _dbcommand.Parameters.Add(params1);
            _dbcommand.Parameters.Add(params2);
            _dbcommand.Parameters.Add(params3);
            _dbcommand.Parameters.Add(params4);
            
            _dbcommand.ExecuteReader();
            _dbcommand.Parameters.Clear();
            
            return RedirectToAction("AnnouncementItem", new RouteValueDictionary(new
            {
                controller = "Announcement", action = "AnnouncementItem", id = announcementId
            }));
        }

        return View(model);
    }

    private List<PlacementType> GetPlacementTypes()
    {
        _dbcommand.CommandText = @"SELECT * FROM Placement_types";

        List<PlacementType> placementTypes = new List<PlacementType>();

        var dataReader = _dbcommand.ExecuteReader();
        while (dataReader.Read())
        {
            placementTypes.Add(new PlacementType
            {
                Id = (Guid)dataReader.GetValue(0),
                PlacementTypeName = (string)dataReader.GetValue(1)
            });
        }
        
        dataReader.Close();

        return placementTypes;
    }

    private List<Facility> GetFacilities()
    {
        _dbcommand.CommandText = @"SELECT * FROM Facilities";

        List<Facility> facilities = new List<Facility>();

        var dataReader = _dbcommand.ExecuteReader();
        while (dataReader.Read())
        {
            facilities.Add(new Facility
            {
                Id = (Guid)dataReader.GetValue(0),
                FacilityName = (string)dataReader.GetValue(1)
            });
        }
        
        dataReader.Close();

        return facilities;
    }

    [HttpGet]
    public IActionResult NewAnnouncement()
    {
        ViewData["PlacementTypes"] = new SelectList(GetPlacementTypes(), "Id", "PlacementTypeName");
        return View(new NewAnnouncementModel
        {
            Facilities = GetFacilities()
        });
    }

    [HttpPost]
    public async Task<IActionResult> NewAnnouncement(NewAnnouncementModel model)
    {
        if (ModelState.IsValid)
        {
            _dbcommand.CommandText = @"CALL add_announcement(
    (@p0),
	(@p1),
	(@p2),
	(@p3),
	(@p4),
	(@p5),
	(@p6),
	(@p7)
)";
            var params0 = _dbcommand.CreateParameter();
            var params1 = _dbcommand.CreateParameter();
            var params2 = _dbcommand.CreateParameter();
            var params3 = _dbcommand.CreateParameter();
            var params4 = _dbcommand.CreateParameter();
            var params5 = _dbcommand.CreateParameter();
            var params6 = _dbcommand.CreateParameter();
            var params7 = _dbcommand.CreateParameter();

            var id = Guid.NewGuid();
            
            params0.ParameterName = "p0";
            params0.Value = id;
            
            params1.ParameterName = "p1";
            params1.Value = User.Identity.Name;
            
            params2.ParameterName = "p2";
            params2.Value = model.Title;
            
            params3.ParameterName = "p3";
            params3.Value = model.Description;
            
            params4.ParameterName = "p4";
            params4.Value = model.Address;
            
            params5.ParameterName = "p5";
            params5.Value = model.RoomCount;
            
            params6.ParameterName = "p6";
            params6.Value = model.PlacementTypeId;
            
            params7.ParameterName = "p7";
            params7.Value = model.PricePerDay;

            _dbcommand.Parameters.Add(params0);
            _dbcommand.Parameters.Add(params1);
            _dbcommand.Parameters.Add(params2);
            _dbcommand.Parameters.Add(params3);
            _dbcommand.Parameters.Add(params4);
            _dbcommand.Parameters.Add(params5);
            _dbcommand.Parameters.Add(params6);
            _dbcommand.Parameters.Add(params7);
            
            _dbcommand.ExecuteReader();
            _dbcommand.Parameters.Clear();

            return RedirectToAction("AllAnnouncements", "Announcement");
        }
        
        ViewData["PlacementTypes"] = new SelectList(GetPlacementTypes(), "Id", "PlacementTypeName");
        return View(model);
    }

    public void AddFacilities(Guid announcementId, Guid? facilityId)
    {
        _dbcommand.CommandText = @"INSERT INTO Announcement_has_Facility VALUES (
	(@p1),
	(@p2)
)";
        var params1 = _dbcommand.CreateParameter();
        var params2 = _dbcommand.CreateParameter();
        
        params1.ParameterName = "p1";
        params1.Value = announcementId;
            
        params2.ParameterName = "p2";
        params2.Value = facilityId;
        
        _dbcommand.Parameters.Add(params1);
        _dbcommand.Parameters.Add(params2);

        _dbcommand.ExecuteReader();
        _dbcommand.Parameters.Clear();
    }

    [HttpGet]
    public IActionResult EditComment(Guid id)
    {
        return View(new EditReviewModel
        {
            Id = id
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditComment(EditReviewModel model)
    {
        if (ModelState.IsValid)
        {
            _dbcommand.CommandText = @"SELECT review, rating FROM Reviews WHERE id = (@p1)";
            
            var params1 = _dbcommand.CreateParameter();
            
            params1.ParameterName = "p1";
            params1.Value = model.Id;
            _dbcommand.Parameters.Add(params1);

            var review = new Review();
            var dataReader = _dbcommand.ExecuteReader();
            while (dataReader.Read())
            {
                review.Comment = (string)dataReader.GetValue(0);
                review.Rating = (int)dataReader.GetValue(1);
            }
            
            _dbcommand.Parameters.Clear();
            dataReader.Close();
            
            review.Comment = model.Comment ?? review.Comment;
            review.Rating = model.Rating == 0 ? review.Rating : model.Rating;
            review.Id = model.Id;
            
            UpdateReview(review);
            
            return RedirectToAction("AllAnnouncements", "Announcement");
        }

        return View(model);
    }

    private void UpdateReview(Review review)
    {
        _dbcommand.CommandText = @"UPDATE Reviews SET review = (@p1), rating = (@p2) WHERE id = (@p3)";
        
        var params1 = _dbcommand.CreateParameter();
        var params2 = _dbcommand.CreateParameter();
        var params3 = _dbcommand.CreateParameter();
        
        params1.ParameterName = "p1";
        params1.Value = review.Comment;
        
        params2.ParameterName = "p2";
        params2.Value = review.Rating;
        
        params3.ParameterName = "p3";
        params3.Value = review.Id;
        
        _dbcommand.Parameters.Add(params1);
        _dbcommand.Parameters.Add(params2);
        _dbcommand.Parameters.Add(params3);
        
        _dbcommand.ExecuteReader();
        _dbcommand.Parameters.Clear();
    }

    public async Task<IActionResult> DeleteReview(Guid id, string returnUrl)
    {
        _dbcommand.CommandText = @"DELETE FROM Reviews WHERE id = (@p1)";
        
        var params1 = _dbcommand.CreateParameter();
        
        params1.ParameterName = "p1";
        params1.Value = id;
        
        _dbcommand.Parameters.Add(params1);
        
        _dbcommand.ExecuteReader();
        _dbcommand.Parameters.Clear();
        
        return Redirect(returnUrl);
    }
}