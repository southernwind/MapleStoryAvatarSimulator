using MapleStoryDB;

using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Text.RegularExpressions;

using WzComparerR2.WzLib;

namespace WzExtractor {
	class Item {
		public static void Extract(DbContextOptions options) {
			var regex = new Regex(@"(^begin|^end)(\d*)$");
			var wz = new Wz_Structure();
			wz.Load(@"C:\Nexon\MapleStory\Item.wz");
			using (var db = new MapleStoryDbContext(options)) {
				var eqp = wz.WzNode.Nodes["ThothSearchOption.img"].GetNodeWzImage();
				if (!eqp.TryExtract()) {
					throw new Exception();
				}
				db.ItemCategories.AddRange(new[]{
					new ItemCategory {
						ItemCategory1 = "Character",
						ItemCategory2 = "Body",
						ItemCategoryName = "肌",
						ItemCategoryRangeBegin = 2000,
						ItemCategoryRangeEnd = 2099
					},
					new ItemCategory {
						ItemCategory1 = "Character",
						ItemCategory2 = "Head",
						ItemCategoryName = "肌(顔)",
						ItemCategoryRangeBegin = 12000,
						ItemCategoryRangeEnd = 12099
					},
					new ItemCategory {
						ItemCategory1 = "Character",
						ItemCategory2 = "Hair",
						ItemCategoryName = "髪",
						ItemCategoryRangeBegin = 30000,
						ItemCategoryRangeEnd = 49999
					},
					new ItemCategory {
						ItemCategory1 = "Character",
						ItemCategory2 = "Face",
						ItemCategoryName = "顔",
						ItemCategoryRangeBegin = 20000,
						ItemCategoryRangeEnd = 29999
					},
				});

				foreach (var category1 in eqp.Node.Nodes["ItemDetailCategory"].Nodes) {
					foreach (var category2 in category1.Nodes) {
						var ranges =
							category2
								.Nodes
								.Where(x => regex.IsMatch(x.Text))
								.GroupBy(x => regex.Replace(x.Text, "$2"))
								.Select(x => {
									int? begin = null;
									int? end = null;
									foreach (var attr in x) {
										if (attr.Text.StartsWith("begin")) {
											begin = GetValue<int>(attr.Value);
										}
										if (attr.Text.StartsWith("end")) {
											end = GetValue<int>(attr.Value);
										}
									}
									return (begin, end);
								});
						foreach (var (begin, end) in ranges) {
							var category = new ItemCategory {
								ItemCategory1 = category1.Text,
								ItemCategory2 = category2.Text,
								ItemCategoryName = GetValue<string>(category2.Nodes["string"].Value),
								ItemCategoryRangeBegin = (int)begin,
								ItemCategoryRangeEnd = (int)end
							};

							db.ItemCategories.Add(category);
						}

					}
				}
				db.SaveChanges();
			}
		}

		/// <summary>
		/// object型変換
		/// </summary>
		/// <typeparam name="T">変換後型</typeparam>
		/// <param name="value">変換前値</param>
		/// <returns>変換後値</returns>
		private static T GetValue<T>(object value) {
			if (value is T t) {
				return t;
			}

			if (value is string str) {
				if (typeof(int) == typeof(T)) {
					return (dynamic)int.Parse(str);
				}
			}

			return (dynamic)value;
		}
	}
}
