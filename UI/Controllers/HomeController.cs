namespace UI.Controllers
{
    public class HomeController : Controller
    {
        
        public HomeController()
        {
            
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
