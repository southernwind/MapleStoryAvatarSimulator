using MapleStoryDB;

using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;

using WzComparerR2.WzLib;

namespace WzExtractor {
	class Base {
		public static void Extract(DbContextOptions options) {
			var wz = new Wz_Structure();
			wz.Load(@"C:\Nexon\MapleStory\Base.wz");
			using (var db = new MapleStoryDbContext(options)) {
				var zmap = wz.WzNode.Nodes["zmap.img"].GetNodeWzImage();
				if (!zmap.TryExtract()) {
					throw new Exception();
				}
				foreach (var (value, index) in zmap.Node.Nodes.Reverse().Select((value, index) => (value, index))) {
					db.Zmaps.Add(new Zmap() {
						Name = value.Text,
						Index = index
					});
				}

				var smap = wz.WzNode.Nodes["smap.img"].GetNodeWzImage();
				if (!smap.TryExtract()) {
					throw new Exception();
				}

				foreach (var (value, index) in smap.Node.Nodes.Reverse().Select((value, index) => (value, index))) {
					db.Smaps.Add(new Smap() {
						Key = value.Text,
						Value = value.GetValue<string>(),
						Index = index
					});
				}
				db.SaveChanges();
			}
		}
	}
}
