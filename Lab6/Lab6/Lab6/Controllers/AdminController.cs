using Lab6.Entities;
using Lab6.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Lab6.Controllers;

public class AdminController: Controller
{
    private readonly NpgsqlCommand _dbcommand;

    public AdminController()
    {
        _dbcommand = DBManager.getCommand();
    }

    public IActionResult MainPage()
    {
        return View();
    }
    
    public IActionResult AnnouncementLogs()
    {
        return View(GetAnnouncementLogs());
    }

    public AnnouncementLogModel GetAnnouncementLogs()
    {
        _dbcommand.CommandText = @"SELECT announcement_id, user_id, event FROM Announcement_log";

        var model = new AnnouncementLogModel();
        model.AnnouncementLogs = new List<AnnouncementLog>();
        
        var dataReader = _dbcommand.ExecuteReader();
        while (dataReader.Read())
        {
            var temp = new AnnouncementLog();
            temp.AnnouncementId = (Guid)dataReader.GetValue(0);
            temp.UserId = (Guid)dataReader.GetValue(1);
            temp.Event = (string)dataReader.GetValue(2);
            
            model.AnnouncementLogs.Add(temp);
        }
        
        dataReader.Close();
        return model;
    }
    
    public IActionResult ReviewLogs()
    {
        return View(GetReviewLogs());
    }

    public ReviewLogModel GetReviewLogs()
    {
        _dbcommand.CommandText = @"SELECT Review_log.review_id, Review_log.user_id, Review_log.event, Reviews.announcement_id 
FROM Review_log
LEFT JOIN Reviews ON Reviews.id = Review_log.review_id";

        var model = new ReviewLogModel();
        model.ReviewLogs = new List<ReviewLog>();
        
        var dataReader = _dbcommand.ExecuteReader();
        while (dataReader.Read())
        {
            var temp = new ReviewLog();
            temp.ReviewId = (Guid)dataReader.GetValue(0);
            temp.UserId = (Guid)dataReader.GetValue(1);
            temp.Event = (string)dataReader.GetValue(2);
            temp.AnnouncementId = (Guid)dataReader.GetValue(3);
            
            model.ReviewLogs.Add(temp);
        }
        
        dataReader.Close();
        return model;
    }
}