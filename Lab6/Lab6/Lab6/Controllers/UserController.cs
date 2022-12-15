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
}