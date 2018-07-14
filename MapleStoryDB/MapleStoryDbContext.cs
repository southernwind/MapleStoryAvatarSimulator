using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;

namespace MapleStoryDB {
	public class MapleStoryDbContext : DbContext {
		public DbSet<Equipment> Equipments {
			get;
			set;
		}

		public DbSet<Frame> Frames {
			get;
			set;
		}

		public DbSet<FrameImage> FrameImages {
			get;
			set;
		}

		public DbSet<Image> Images {
			get;
			set;
		}

		public DbSet<Map> Maps {
			get;
			set;
		}

		public DbSet<ItemCategory> ItemCategories {
			get;
			set;
		}

		public DbSet<Zmap> Zmaps {
			get;
			set;
		}
		public DbSet<Smap> Smaps {
			get;
			set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MapleStoryDbContext(DbContextOptions options) : base(options) {
		}

		/// <summary>
		/// テーブル設定
		/// </summary>
		/// <param name="modelBuilder"></param>
		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			// 主キー定義
			modelBuilder.Entity<Equipment>().HasKey(eq => new {
				eq.EquipmentId
			});

			modelBuilder.Entity<Frame>().HasKey(frame => new {
				frame.EquipmentId,
				frame.Motion,
				frame.Pattern,
				frame.FrameNo,
			});

			modelBuilder.Entity<FrameImage>().HasKey(fi => new {
				fi.EquipmentId,
				fi.Motion,
				fi.Pattern,
				fi.FrameNo,
				fi.FullPath
			});

			modelBuilder.Entity<Image>().HasKey(image => new {
				image.ImageLogicalPath,
			});

			modelBuilder.Entity<Map>().HasKey(map => new {
				map.ImageLogicalPath,
				map.BasedPositionName
			});

			modelBuilder.Entity<ItemCategory>().HasKey(ic => new {
				ic.ItemCategory1,
				ic.ItemCategory2,
				ic.ItemCategoryRangeBegin
			});

			modelBuilder.Entity<Zmap>().HasKey(z => new {
				z.Name
			});

			modelBuilder.Entity<Smap>().HasKey(s => new {
				s.Key
			});

			// 外部キー定義
			modelBuilder.Entity<Equipment>()
				.HasMany(eq => eq.Frames)
				.WithOne(frame => frame.Equipment)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Frame>()
				.HasMany(frame => frame.FrameImages)
				.WithOne(image => image.Frame)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Image>()
				.HasMany(image => image.FrameImages)
				.WithOne(fi => fi.Image)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Image>()
				.HasMany(image => image.Maps)
				.WithOne(map => map.Image)
				.OnDelete(DeleteBehavior.Cascade);

		}
	}

	/// <summary>
	/// ログ出力クラス
	/// </summary>
	public class MapleStoryDbLoggerProvider : ILoggerProvider {
		public ILogger CreateLogger(string categoryName) {
			return new ConsoleLogger();
		}

		public void Dispose() {
		}

		private class ConsoleLogger : ILogger {
			public IDisposable BeginScope<TState>(TState state) {
				return null;
			}

			public bool IsEnabled(LogLevel logLevel) {
				return true;
			}

			public void Log<TState>(
				LogLevel logLevel,
				EventId eventId,
				TState state,
				Exception exception,
				Func<TState, Exception, string> formatter) {
				Debug.WriteLine(formatter(state, exception));
			}
		}
	}
}
