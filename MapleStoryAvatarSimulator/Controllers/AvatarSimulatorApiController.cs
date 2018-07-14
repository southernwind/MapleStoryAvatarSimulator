using MapleStoryDB;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MapleStoryAvatarSimulator.Controllers {
	[Produces("application/json")]
	[Route("api/[action]")]
	public class AvatarSimulatorApiController : Controller {
		private MapleStoryDbContext _context;
		public AvatarSimulatorApiController(MapleStoryDbContext context) {
			this._context = context;
		}

		/// <summary>
		/// アイコンリストの読み込み
		/// 読み込み範囲で指定されたID間のアイコンを表示するためのアイテム情報を返却する
		/// </summary>
		/// <param name="rangeStart">読み込み範囲</param>
		/// <param name="rangeEnd">読み込み範囲</param>
		/// <returns></returns>
		[Route("{rangeStart}-{rangeEnd}")]
		public async Task<JsonResult> GetIcons(int rangeStart, int rangeEnd) {

			// イマは大げさなSQLを発行しているけれど、最終的にはiconがないものはiconを事前生成しておく方が良いかもしれない

			// 表情でイメージが変わる部位
			var target = new[] {
				"Fc",
				"Af"
			};
			var query =
				this._context
					.Equipments
					.Where(x => rangeStart <= x.EquipmentId && x.EquipmentId <= rangeEnd)
					.Join(
						this._context.Frames,
						e => new { e.EquipmentId },
						f => new { f.EquipmentId },
						(equipment, frame) => new { equipment, frame })
					.Where(x =>
						(x.frame.Motion == "stand1" && !target.Contains(x.equipment.Islot)) ||
						(x.frame.Motion == "default" && target.Contains(x.equipment.Islot))
					).Join(
						this._context.FrameImages,
						ef => new { ef.frame.EquipmentId, ef.frame.Motion, ef.frame.Pattern, ef.frame.FrameNo },
						fi => new { fi.EquipmentId, fi.Motion, fi.Pattern, fi.FrameNo },
						(ef, frameImage) => new { ef.equipment, ef.frame, frameImage })
					.Join(
						this._context.Images,
						effi => effi.frameImage.ImageLogicalPath,
						i => i.ImageLogicalPath,
						(effi, image) => new { effi.equipment, effi.frame, effi.frameImage, image })
					.Join(
						this._context.Maps,
						effii => effii.image.ImageLogicalPath,
						m => m.ImageLogicalPath,
						(effii, map) => new { effii.equipment, effii.frame, effii.frameImage, effii.image, map })
					.Join(
						this._context.Zmaps,
						effiim => effiim.image.Z,
						z => z.Name,
						(effiim, z) => new { effiim.equipment, effiim.frame, effiim.frameImage, effiim.image, effiim.map, z })
					.Select(effiimz => new {
						effiimz.equipment.EquipmentName,
						effiimz.equipment.Islot,
						effiimz.equipment.Vslot,
						iconPath = effiimz.equipment.Icon.ImageActualPath,
						effiimz.frame.EquipmentId,
						effiimz.frame.Motion,
						effiimz.frame.Pattern,
						effiimz.frame.FrameNo,
						effiimz.frameImage.ImageLogicalPath,
						effiimz.image.ImageActualPath,
						effiimz.image.OriginX,
						effiimz.image.OriginY,
						effiimz.z.Index,
						effiimz.map.BasedPositionName,
						effiimz.map.X,
						effiimz.map.Y
					})
					.GroupBy(es => es.EquipmentId)
					.ToArrayAsync();

			var result = (await query)
			.Select(e => new {
				id = e.Key,
				name = e.First().EquipmentName,
				e.First().iconPath,
				islot = e.First().Islot,
				vslots = Regex.Split(e.First().Vslot ?? "", "(.{2})").Where(s => s != "").ToArray(),
				loaded = false,
				frames = e.First().iconPath == null ? e.GroupBy(fs => new { fs.EquipmentId, fs.Motion, fs.Pattern, fs.FrameNo })
					.Select(f => new {
						frameNo = f.Key.FrameNo,
						images = f.GroupBy(fis => fis.ImageLogicalPath)
							.Select(fi => new {
								imagePath = fi.First().ImageActualPath,
								originX = fi.First().OriginX,
								originY = fi.First().OriginY,
								zIndex = fi.First().Index,
								maps = fi.Select(m => new {
									basedPositionName = m.BasedPositionName,
									x = m.X,
									y = m.Y
								}).ToArray()
							}).ToArray()
					}).ToArray() : null
			}).ToArray();
			return new JsonResult(result);
		}

		/// <summary>
		/// smap情報を返却する
		/// </summary>
		/// <returns>smap情報</returns>
		public async Task<JsonResult> GetSmaps() {
			var query = this._context.Smaps.OrderByDescending(x => x.Index).ToArrayAsync();

			var result = (await query).Select(x => new {
				name = x.Key,
				vslots = Regex.Split(x.Value ?? "", "(.{2})").Where(s => s != "").ToArray(),
				index = x.Index
			});

			return new JsonResult(result);
		}

		/// <summary>
		/// zmap情報を返却する
		/// </summary>
		/// <returns>zmap情報</returns>
		public async Task<JsonResult> GetZmaps() {
			var query = this._context.Zmaps.OrderByDescending(x => x.Index).ToArrayAsync();

			var result = (await query).Select(x => new {
				name = x.Name,
				index = x.Index
			});

			return new JsonResult(result);
		}

		/// <summary>
		/// アイテムカテゴリー一覧を返却する
		/// </summary>
		/// <returns>アイテムカテゴリー一覧</returns>
		public async Task<JsonResult> GetItemCategories() {
			var query =
				this._context
					.ItemCategories
					.Where(x =>
						x.ItemCategory1 == "Character" ||
						x.ItemCategory1.EndsWith("Weapon") ||
						new[] {
							"Eye",
							"Ear",
							"Face",
							"Cap",
							"Cape",
							"Coat",
							"Glove",
							"Longcoat",
							"Pants",
							"Shield",
							"Shoes"
						}.Contains(x.ItemCategory2)
					)
					.Select(x => new {
						itemCategory1 = x.ItemCategory1,
						itemCategory2 = x.ItemCategory2,
						name = x.ItemCategoryName,
						count = this._context.Equipments.Count(e => x.ItemCategoryRangeBegin <= e.EquipmentId && e.EquipmentId <= x.ItemCategoryRangeEnd),
						rangeBegin = x.ItemCategoryRangeBegin,
						rangeEnd = x.ItemCategoryRangeEnd
					})
					.ToArrayAsync();
			return new JsonResult(await query);
		}

		/// <summary>
		/// アイテム画像を返却する
		/// </summary>
		/// <param name="motion">モーション</param>
		/// <param name="expression">表情</param>
		/// <param name="eqid">アイテムID</param>
		/// <returns>アイテム画像</returns>
		[Route("{motion}/{expression}")]
		public async Task<JsonResult> GetItemImages(string motion, string expression, int[] eqid) {

			// どうもInclude,ThenIncludeを繰り返すとその分だけSELECT文が発行されるらしい
			// なので、パフォーマンスの問題からJoinで結合したクエリを発行してから自力で構造化することにする

			// 表情でイメージが変わる部位
			var target = new[] {
				"Fc",
				"Af"
			};

			var query =
				this._context
					.Equipments
					.Where(x => eqid.Contains(x.EquipmentId))
					.Join(
						this._context.Frames,
						e => new { e.EquipmentId },
						f => new { f.EquipmentId },
						(equipment, frame) => new { equipment, frame })
					.Where(x =>
						(x.frame.Motion == motion && !target.Contains(x.equipment.Islot)) ||
						(x.frame.Motion == expression && target.Contains(x.equipment.Islot))
					).Join(
						this._context.FrameImages,
						ef => new { ef.frame.EquipmentId, ef.frame.Motion, ef.frame.Pattern, ef.frame.FrameNo },
						fi => new { fi.EquipmentId, fi.Motion, fi.Pattern, fi.FrameNo },
						(ef, frameImage) => new { ef.equipment, ef.frame, frameImage })
					.Join(
						this._context.Images,
						effi => effi.frameImage.ImageLogicalPath,
						i => i.ImageLogicalPath,
						(effi, image) => new { effi.equipment, effi.frame, effi.frameImage, image })
					.Join(
						this._context.Maps,
						effii => effii.image.ImageLogicalPath,
						m => m.ImageLogicalPath,
						(effii, map) => new { effii.equipment, effii.frame, effii.frameImage, effii.image, map })
					.Join(
						this._context.Zmaps,
						effiim => effiim.image.Z,
						z => z.Name,
						(effiim, z) => new { effiim.equipment, effiim.frame, effiim.frameImage, effiim.image, effiim.map, z })
					.Select(effiimz => new {
						effiimz.equipment.EquipmentName,
						effiimz.equipment.Islot,
						effiimz.equipment.Vslot,
						iconPath = effiimz.equipment.Icon.ImageActualPath,
						effiimz.frame.EquipmentId,
						effiimz.frame.Motion,
						effiimz.frame.Pattern,
						effiimz.frame.FrameNo,
						effiimz.frameImage.ImageLogicalPath,
						effiimz.image.ImageActualPath,
						effiimz.image.OriginX,
						effiimz.image.OriginY,
						effiimz.z.Index,
						effiimz.z.Name,
						effiimz.map.BasedPositionName,
						effiimz.map.X,
						effiimz.map.Y
					})
					.GroupBy(es => es.EquipmentId)
					.ToArrayAsync();

			var result = (await query)
			.Select(e => new {
				id = e.Key,
				name = e.First().EquipmentName,
				e.First().iconPath,
				islot = e.First().Islot,
				vslots = Regex.Split(e.First().Vslot ?? "", "(.{2})").Where(s => s != "").ToArray(),
				loaded = true,
				frames = e.GroupBy(fs => new { fs.EquipmentId, fs.Motion, fs.Pattern, fs.FrameNo })
					.Select(f => new {
						frameNo = f.Key.FrameNo,
						images = f.GroupBy(fis => fis.ImageLogicalPath)
							.Select(fi => new {
								imagePath = fi.First().ImageActualPath,
								originX = fi.First().OriginX,
								originY = fi.First().OriginY,
								zIndex = fi.First().Index,
								z = fi.First().Name,
								maps = fi.Select(m => new {
									basedPositionName = m.BasedPositionName,
									x = m.X,
									y = m.Y
								}).ToArray()
							}).ToArray()
					}).ToArray()
			}).ToArray();
			return new JsonResult(result);
		}
	}
}