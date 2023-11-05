using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using eCode.Models;

namespace eCode.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new API().ListarDesafiosHome());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}