using Microsoft.AspNetCore.Mvc;
using Leave__Management_System.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Leave__Management_System.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            var data = new TestViewModel
            {
                Name = "Test",
                DateofBirth = new DateTime(2000,01,01)
            };
           return View(data);
        }
        
    }
}
