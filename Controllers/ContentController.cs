using Microsoft.AspNetCore.Mvc;

namespace eCode.Controllers
{
    public class ContentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
