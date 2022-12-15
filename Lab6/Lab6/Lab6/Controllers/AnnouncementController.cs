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
}