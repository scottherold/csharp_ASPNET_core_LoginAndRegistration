using System;
using Microsoft.AspNetCore.Mvc;
using LoginAndRegistration.Models;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LoginAndRegistration.Controllers
{
    public class MainController : Controller
    {
        // Sets up the context class
        private UserContext dbContext;

        // Creates a new context object when the controller is accessed
        public MainController(UserContext context)
        {
            dbContext = context;
        }
        
        // Default route displays the Index/Login page
        [HttpGet("")]
        public IActionResult Index()
        {
            // Retrieves data from session to query as an event handler
            // checks to see if the session data is present to prevent 
            // penetration.
            string LoggedIn = HttpContext.Session.GetString("LoggedIn");
            int? userId = HttpContext.Session.GetInt32("UserId");
            string email = HttpContext.Session.GetString("Email");

            // If logged in not present, proceed to default
            if (LoggedIn == null)
            {
                return View("Index");
            }
            else
            {
                // Checks to see if the user is in the DB
                User userInDb = dbContext.Users.FirstOrDefault(user => user.Email == email);
                if(userInDb == null)
                {
                    // if user is not in DB, kill session, redirect to index
                    HttpContext.Session.Clear();
                    return RedirectToAction("Index");
                }
                else
                {
                    // Checks to see if session user ID matches the actuall user ID
                    if(userInDb.UserId != (int)userId)
                    {
                        // if ID's do not match, kills session, redirects to Index
                        HttpContext.Session.Clear();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Success");
                    }
                }
            }
        }
        
        // Get route displays registration page
        [HttpGet("new")]
        public IActionResult Register()
        {
            // Checks to see if user is logged in
            string LoggedIn = HttpContext.Session.GetString("LoggedIn");
            
            // If user is not logged in proceed
            if(LoggedIn == null)
            {
                return View("Register");
            }
            else
            {
                // If user is logged in, logout user and proceed
                HttpContext.Session.Clear();
                return View("Register");
            }
        }
        
        // Get route displays succes page post succesful registration/login
        [HttpGet("success")]
        public IActionResult Success()
        {
            // Retrieves data from session to query as an event handler
            // checks to see if the session data is present to prevent 
            // penetration.
            string LoggedIn = HttpContext.Session.GetString("LoggedIn");
            int? userId = HttpContext.Session.GetInt32("UserId");
            string email = HttpContext.Session.GetString("Email");

            // If logged in not present, proceed to default
            if (LoggedIn == null)
            {
                return View("Index");
            }
            else
            {
                // Checks to see if the user is in the DB
                User userInDb = dbContext.Users.FirstOrDefault(user => user.Email == email);
                if(userInDb == null)
                {
                    // if user is not in DB, kill session, redirect to index
                    HttpContext.Session.Clear();
                    return RedirectToAction("Index");
                }
                else
                {
                    // Checks to see if session user ID matches the actuall user ID
                    if(userInDb.UserId != (int)userId)
                    {
                        // if ID's do not match, kills session, redirects to Index
                        HttpContext.Session.Clear();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View("Success", userInDb);
                    }
                }
            }
        }
        
        // Post route for creating a new user
        [HttpPost("create")]
        public IActionResult Create(User user)
        {
            // checks sanitization from Model
            if(ModelState.IsValid)
            {
                // Checks to see if there is a duplicate email address
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    // Manually adds a ModelState error to the Email field
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Register");
                }
                else
                {
                    // Sets DateTime variable for DB addition
                    user.CreatedAt = DateTime.Now;
                    user.UpdatedAt = DateTime.Now;

                    // Hashes password
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    
                    // Adds user to the DB.
                    dbContext.Add(user);
                    dbContext.SaveChanges();

                    // Creates 'Logged In' status, with security validation.
                    // Each route can now check to see if the User is logged in using
                    // session data to validate a query to the DB. If the email does 
                    // not match the email for the user id, then session will be cleared.
                    HttpContext.Session.SetString("LoggedIn", "Yes");
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("Email", user.Email);

                    return RedirectToAction("Success");
                }
            }
            else
            {
                return View("Register");
            }
        }
        
        // Post route for logging in
        [HttpPost("login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            // Checks sanitization from Model
            if(ModelState.IsValid)
            {
                // If initial ModelState is valid, query for a user with provided email
                var loginUser = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);

                // If user exists with provided email, else proceed with login
                if(loginUser == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Password", "Invalid Email/Password");
                    return RedirectToAction("Index");
                }
                else
                {
                    // Initializes hasher object
                    var hasher = new PasswordHasher<LoginUser>();

                    // Verify provided password against hash stored in db
                    var result = hasher.VerifyHashedPassword(userSubmission, loginUser.Password, userSubmission.Password);

                    // Result can be compared to 0 for failure
                    if(result == 0)
                    {
                        ModelState.AddModelError("Password", "Invalid Email/Password");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Creates 'Logged In' status, with security validation.
                        // Each route can now check to see if the User is logged in using
                        // session data to validate a query to the DB. If the email does 
                        // not match the email for the user id, then session will be cleared.
                        HttpContext.Session.SetString("LoggedIn", "Yes");
                        HttpContext.Session.SetInt32("UserId", loginUser.UserId);
                        HttpContext.Session.SetString("Email", loginUser.Email);
                        return RedirectToAction("Success");
                    }
                }
            }
            else
            {
                return View("Index");
            }
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}