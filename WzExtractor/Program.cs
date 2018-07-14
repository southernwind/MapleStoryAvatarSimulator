
using MapleStoryDB;

using Microsoft.EntityFrameworkCore;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Configuration;

namespace WzExtractor {
	class Program {
		private static List<(string, Type)> list = new List<(string, Type)>();

		public static int Main(string[] args) {
			var optionsBuilder = new DbContextOptionsBuilder();
			optionsBuilder.UseMySql(new MySqlConnectionStringBuilder {
				Server = ConfigurationManager.AppSettings["server"],
				Port = uint.Parse(ConfigurationManager.AppSettings["port"]),
				UserID = ConfigurationManager.AppSettings["user"],
				Password = ConfigurationManager.AppSettings["password"],
				Database = ConfigurationManager.AppSettings["database"],
				CharacterSet = "utf8_bin",
			}.ToString());

			Console.WriteLine("データベース初期化開始");
			using (var db = new MapleStoryDbContext(optionsBuilder.Options)) {
				db.Database.EnsureDeleted();
				db.Database.EnsureCreated();
				var sql = $"ALTER TABLE `{nameof(db.FrameImages)}` CHANGE COLUMN `{nameof(FrameImage.FullPath)}` `{nameof(FrameImage.FullPath)}` VARCHAR(255) NOT NULL COLLATE 'utf8_bin' AFTER `{nameof(FrameImage.FrameNo)}`;";
				db.Database.ExecuteSqlCommand(sql);
			}

			Console.WriteLine("データベース初期化完了");
			Console.WriteLine("Character開始");
			Character.Extract(optionsBuilder.Options);

			Console.WriteLine("Character完了");
			Console.WriteLine("String開始");
			String.Extract(optionsBuilder.Options);
			Console.WriteLine("String完了");
			Console.WriteLine("Item開始");
			Item.Extract(optionsBuilder.Options);
			Console.WriteLine("Item完了");

			Console.WriteLine("Base開始");
			Base.Extract(optionsBuilder.Options);
			Console.WriteLine("Base完了");

			Console.WriteLine("完了");
			Console.ReadLine();
			Console.ReadLine();
			Console.ReadLine();
			Console.ReadLine();
			Console.ReadLine();
			Console.ReadLine();
			Console.ReadLine();
			Console.ReadLine();
			Console.ReadLine();

			return 0;
		}

	}
}
