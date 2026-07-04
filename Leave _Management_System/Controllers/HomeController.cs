using Leave__Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Leave__Management_System.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
           
            if (!(User?.Identity?.IsAuthenticated ?? false))
            {
                return View("_Landing");
            }

            return View();
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {

            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(model);
        }
    }
}
