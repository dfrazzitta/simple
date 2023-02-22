using Microsoft.AspNetCore.Mvc;

namespace Simple.Controllers
{

    public class Listen81Controller : Controller
    {
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
