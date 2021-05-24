using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WIL_Protoype.Models;
using System.Data.SqlClient;


namespace WIL_Protoype.Controllers
{
    public class HomeController : Controller
    {
        

        private readonly ILogger<HomeController> _logger;
 


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login(string returnURL)
        {
            ViewData["ReturnURL"] = returnURL;
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Validate(string email , string password , string returnURL)
        {

            string dbEmail = "jack@gmail.com",
                      dbPassword = "password1",
                      dbFirstName = "",
                      dbLastName = "";


            //connect to database and get dbEmail, dbFirstName , dbLastName and dbPassword Values

            string ConnectionString = "Data Source=playtimedatabase.database.windows.net;Initial Catalog=PlaytimeDatabase;Persist Security Info=True;User ID=adminplaytime;Password=***********";

                ViewData["ReturnUrl"] = returnURL;

            if (email == dbEmail && password == dbPassword)
            {
                var claims = new List<Claim>(); //claims are properties that describe a user. e.g. First Name, Last Name, Age 

                claims.Add(new Claim(ClaimTypes.NameIdentifier, dbFirstName));
                claims.Add(new Claim(ClaimTypes.Email, email));
                claims.Add(new Claim(ClaimTypes.Name, dbFirstName + " " + dbLastName)); ;


                var claimsId = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var claimsPrincipal = new ClaimsPrincipal(claimsId); //authentication ticket which will be stored in a cookie 
                await HttpContext.SignInAsync(claimsPrincipal);

                return Redirect(returnURL);  //login successful 
            }
            else
            {
                //login unsuccessful
                TempData["Error"] = "Error: Invalid Credentials";
                return View("login");
            }
        }


        [Authorize] //need to be authorized in order to log out 
        public async Task <IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return Redirect("/"); //return to home page 
        }

        [Authorize]
        public IActionResult Secured()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
