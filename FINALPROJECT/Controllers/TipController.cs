using FINALPROJECT.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FINALPROJECT.Controllers
{
    [Route("tips")]
    public class TipController : Controller
    {
        private readonly ITipRepository _tips;
        public TipController(ITipRepository tips) => _tips = tips;

        [HttpGet("")]
        public IActionResult Index()
        {
            var tips = _tips.GetAll();
            return View(tips);
        }
    }
}
