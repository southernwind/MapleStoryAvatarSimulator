using MapleStoryDB;

using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;

using WzComparerR2.WzLib;

namespace WzExtractor {
	class String {
		public static void Extract(DbContextOptions options) {
			var wz = new Wz_Structure();
			wz.Load(@"C:\Nexon\MapleStory\String.wz");
			using (var db = new MapleStoryDbContext(options)) {
				var eqp = wz.WzNode.Nodes["Eqp.img"].GetNodeWzImage();
				if (!eqp.TryExtract()) {
					throw new Exception();
				}
				foreach (var dir in eqp.Node.Nodes.SelectMany(x => x.Nodes)) {
					foreach (var item in dir.Nodes) {
						var eq = db.Equipments.SingleOrDefault(x => x.EquipmentId == int.Parse(item.Text));
						if (eq != null) {
							eq.EquipmentName = (string)item.Nodes["name"]?.Value;
						}
					}
				}
				db.SaveChanges();
			}
		}
	}
}
