using Microsoft.AspNetCore.Mvc;

namespace NaderCms.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class adminHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
