using MapleStoryDB;

using Microsoft.AspNetCore.Mvc;

namespace MapleStoryAvatarSimulator.Controllers {
	public class MainController : Controller {

		private MapleStoryDbContext _context;
		public MainController(MapleStoryDbContext context) {
			this._context = context;
		}

		public IActionResult Index() {
			return View(this._context.Equipments);
		}
	}
}