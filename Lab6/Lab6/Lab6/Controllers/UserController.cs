using System.Security.Claims;
using Lab6.Entities;
using Lab6.Models.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Lab6.Controllers;

public class UserController : Controller
{
    private readonly NpgsqlCommand _dbcommand;

    public UserController()
    {
        _dbcommand = DBManager.getCommand();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var role = "";
        if (ModelState.IsValid)
        {
            if (CheckForLogin(model.Email, model.Password, out role))
            {
                await SignInUser(model.Email);
                if (role == "client")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (role == "admin")
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            if (CheckForRegister(model.Email))
            {
                await RegisterUser(model);
                return RedirectToAction("Index", "Home");
            }
        }
        return View(model);
    }

    private bool CheckForLogin(string email, string password, out string? role)
    {
        _dbcommand.CommandText = @"SELECT * FROM Users 
        JOIN Roles ON Users.role_id = Roles.id WHERE Users.email = (@p1) AND Users.password = (@p2)";
        var params1 = _dbcommand.CreateParameter();
        var params2 = _dbcommand.CreateParameter();

        params1.ParameterName = "p1";
        params1.Value = email;

        params2.ParameterName = "p2";
        params2.Value = password;

        _dbcommand.Parameters.Add(params1);
        _dbcommand.Parameters.Add(params2);

        var dataReader = _dbcommand.ExecuteReader();

        if (dataReader.Read() == false)
        {
            role = "";
            _dbcommand.Parameters.Clear();
            return false;
        }
        
        role = dataReader.GetValue(dataReader.GetOrdinal("role_name")).ToString();
        _dbcommand.Parameters.Clear();
        return true;
    }

    private bool CheckForRegister(string email)
    {
        _dbcommand.CommandText = @"SELECT * FROM Users WHERE Users.email = (@p1)";
        var params1 = _dbcommand.CreateParameter();
        
        params1.ParameterName = "p1";
        params1.Value = email;
        
        _dbcommand.Parameters.Add(params1);
        
        var dataReader = _dbcommand.ExecuteReader();

        if (dataReader.Read() == false)
        {
            dataReader.Close();
            _dbcommand.Parameters.Clear();
            return true;
        }
        
        _dbcommand.Parameters.Clear();
        return false;
    }

    private async Task SignInUser(string email)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, email),
        };
        var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
    }

    public string GetRole(string email)
    {
        _dbcommand.CommandText = @"SELECT * FROM Users 
    JOIN Roles ON Users.role_id = Roles.id AND Users.email = (@p1)";
        var params1 = _dbcommand.CreateParameter();

        params1.ParameterName = "p1";
        params1.Value = email;

        _dbcommand.Parameters.Add(params1);
        
        var dataReader = _dbcommand.ExecuteReader();

        var role = "";
        if (dataReader.Read() == false)
        {
            role = "";
            _dbcommand.Parameters.Clear();
            return role;
        }

        role = dataReader.GetValue(dataReader.GetOrdinal("role_name")).ToString();
        _dbcommand.Parameters.Clear();

        return role;
    }

    private async Task RegisterUser(RegisterModel model)
    {
        _dbcommand.CommandText = @"CALL register_client((@p1), (@p2), (@p3))";
        var params1 = _dbcommand.CreateParameter();
        var params2 = _dbcommand.CreateParameter();
        var params3 = _dbcommand.CreateParameter();
        
        params1.ParameterName = "p1";
        params1.Value = model.Username;
        
        params2.ParameterName = "p2";
        params2.Value = model.Email;
        
        params3.ParameterName = "p3";
        params3.Value = model.Password;
        
        _dbcommand.Parameters.Add(params1);
        _dbcommand.Parameters.Add(params2);
        _dbcommand.Parameters.Add(params3);
        
        _dbcommand.ExecuteReader();
        _dbcommand.Parameters.Clear();

        await SignInUser(model.Email);
    }
    
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public Guid GetIdFromEmail(string email)
    {
        _dbcommand.CommandText = @"SELECT id FROM Users WHERE email = (@p1)";
        
        var params1 = _dbcommand.CreateParameter();
        
        params1.ParameterName = "p1";
        params1.Value = email;
        _dbcommand.Parameters.Add(params1);
        
        var dataReader = _dbcommand.ExecuteReader();
        var id = Guid.Empty;

        while (dataReader.Read())
        {
            id = (Guid)dataReader.GetValue(0);
        }

        _dbcommand.Parameters.Clear();
        dataReader.Close();
        
        return id;
    }
    
    public IActionResult UserProfile(Guid id)
    {
        return View(GetUser(id));
    }
    
    public UserProfileModel GetUser(Guid id)
    {
        _dbcommand.CommandText = @"SELECT Users.id, Users.username, Users.email, Roles.role_name FROM Users 
	JOIN Roles ON Roles.id = Users.role_id 
	WHERE Users.id = (@p1)";
        var params1 = _dbcommand.CreateParameter();
        
        params1.ParameterName = "p1";
        params1.Value = id;
        
        _dbcommand.Parameters.Add(params1);

        var user = new User();
        var dataReader = _dbcommand.ExecuteReader();
        while (dataReader.Read())
        {
            user.Id = (Guid)dataReader.GetValue(0);
            user.Username = (string)dataReader.GetValue(1);
            user.Email = (string)dataReader.GetValue(2);
            user.RoleName = (string)dataReader.GetValue(3);
        }
        
        dataReader.Close();
        _dbcommand.Parameters.Clear();

        user.Announcements = GetAnnouncements(id);
        user.Reviews = GetReviews(id);
        user.Reservations = GetReservations(id);

        return new UserProfileModel
        {
            User = user
        };
    }

    public List<Announcement> GetAnnouncements(Guid id)
    {
        _dbcommand.CommandText = @"SELECT id, title, address FROM Announcements
	WHERE Announcements.user_id = (@p1)";
        var params1 = _dbcommand.CreateParameter();
        
        params1.ParameterName = "p1";
        params1.Value = id;
        
        _dbcommand.Parameters.Add(params1);

        List<Announcement> announcements = new List<Announcement>();
        var dataReader = _dbcommand.ExecuteReader();
        while (dataReader.Read())
        {
            announcements.Add(new Announcement
            {
                Id = (Guid)dataReader.GetValue(0),
                Title = (string)dataReader.GetValue(1),
                Address = (string)dataReader.GetValue(2)
            });
        }
        
        dataReader.Close();
        _dbcommand.Parameters.Clear();

        return announcements;
    }

    public List<Review> GetReviews(Guid id)
    {
        _dbcommand.CommandText = @"SELECT id, rating, post_date, announcement_id FROM Reviews
	WHERE Reviews.user_id = (@p1)";
        var params1 = _dbcommand.CreateParameter();
        
        params1.ParameterName = "p1";
        params1.Value = id;
        
        _dbcommand.Parameters.Add(params1);

        List<Review> reviews = new List<Review>();
        var dataReader = _dbcommand.ExecuteReader();
        while (dataReader.Read())
        {
            reviews.Add(new Review
            {
                Id = (Guid)dataReader.GetValue(0),
                Rating = (int)dataReader.GetValue(1),
                PostDate = (DateTime)dataReader.GetValue(2),
                AnnouncementId = (Guid)dataReader.GetValue(3)
            });
        }
        
        dataReader.Close();
        _dbcommand.Parameters.Clear();
        
        return reviews;
    }

    public List<Reservation> GetReservations(Guid id)
    {
        _dbcommand.CommandText = @"SELECT id, from_date, till_date, announcement_id FROM Reservations
WHERE user_id = (@p1)";
        
        var params1 = _dbcommand.CreateParameter();
        
        params1.ParameterName = "p1";
        params1.Value = id;
        
        _dbcommand.Parameters.Add(params1);

        List<Reservation> reservations = new List<Reservation>();
        var dataReader = _dbcommand.ExecuteReader();
        while (dataReader.Read())
        {
            reservations.Add(new Reservation
            {
                Id = (Guid)dataReader.GetValue(0),
                FromDate = (DateTime)dataReader.GetValue(1),
                TillDate = (DateTime)dataReader.GetValue(2),
                AnnouncementId = (Guid)dataReader.GetValue(3)
            });
        }
        
        dataReader.Close();
        _dbcommand.Parameters.Clear();
        
        return reservations;
    }

    [HttpGet]
    public IActionResult EditProfile()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(EditProfileModel model)
    {
        if (ModelState.IsValid)
        {
            _dbcommand.CommandText = @"SELECT username, password FROM Users WHERE email = (@p1)";
            
            var params1 = _dbcommand.CreateParameter();
        
            params1.ParameterName = "p1";
            params1.Value = User.Identity.Name;
        
            _dbcommand.Parameters.Add(params1);

            var user = new User();
            var dataReader = _dbcommand.ExecuteReader();
            while (dataReader.Read())
            {
                user.Username = (string)dataReader.GetValue(0);
                user.Password = (string)dataReader.GetValue(1);
            }
            
            _dbcommand.Parameters.Clear();
            dataReader.Close();
            
            user.Username = model.Username ?? user.Username;
            user.Password = model.Password ?? user.Password;
            
            UpdateUser(user);

            return RedirectToAction("AllAnnouncements", "Announcement");
        }
        return View(model);
    }

    private void UpdateUser(User user)
    {
        _dbcommand.CommandText = @"UPDATE Users SET username = (@p1), password = (@p2) WHERE email = (@p3)";
        
        var params1 = _dbcommand.CreateParameter();
        var params2 = _dbcommand.CreateParameter();
        var params3 = _dbcommand.CreateParameter();
        
        params1.ParameterName = "p1";
        params1.Value = user.Username;
        
        params2.ParameterName = "p2";
        params2.Value = user.Password;
        
        params3.ParameterName = "p3";
        params3.Value = User.Identity.Name;
        
        _dbcommand.Parameters.Add(params1);
        _dbcommand.Parameters.Add(params2);
        _dbcommand.Parameters.Add(params3);
        
        _dbcommand.ExecuteReader();
        _dbcommand.Parameters.Clear();
    }
}