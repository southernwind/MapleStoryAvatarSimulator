using MapleStoryDB;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using WzComparerR2.WzLib;

namespace WzExtractor {
	class Character {
		public static void Extract(DbContextOptions options) {
			var wz = new Wz_Structure();
			wz.Load(@"C:\Nexon\MapleStory\Character.wz");
			var character = Directory.CreateDirectory("Character");
			Directory.SetCurrentDirectory(character.FullName);

			using (var db = new MapleStoryDbContext(options)) {

				foreach (var img in wz.WzNode.Nodes.Where(x => x.Value is Wz_Image).OrderBy(x => x.Text).Select(x => x.GetNodeWzImage())) {
					var eq = CreateEquipment(img, false);
					if (eq != null) {
						db.Equipments.Add(eq);
					}
				}
				db.SaveChanges();

				var list = new[] {
					"Accessory",
					"Cap",
					"Cape",
					"Coat",
					"Face",
					"Glove",
					"Hair",
					"Longcoat",
					"Pants",
					"Ring",
					"Shield",
					"Shoes",
					"Weapon"
				};
				foreach (var dir in wz.WzNode.Nodes.Where(x => !(x.Value is Wz_Image) && list.Contains(x.Text)).OrderBy(x => x.Text)) {
					Log(dir);
					var fullpathes = new List<string>();
					var referenceFullpathes = new List<string>();
					foreach (var img in dir.Nodes.OrderBy(x => x.Text).Select(x => x.GetNodeWzImage())) {
						var eq = CreateEquipment(img, dir.Text == "Hair");
						if (eq != null) {
							db.Equipments.Add(eq);
							referenceFullpathes.AddRange(eq.Frames.SelectMany(fr => fr.FrameImages.Select(fi => fi.ImageLogicalPath)));
							fullpathes.AddRange(eq.Frames.SelectMany(fr => fr.FrameImages.Select(fi => fi.Image?.ImageLogicalPath)));
						}
					}
					foreach (var i in referenceFullpathes.Except(fullpathes)) {
						Console.WriteLine(i);
					}
					db.SaveChanges();
				}
			}
		}

		private static Equipment CreateEquipment(Wz_Image img, bool frameIncludeSubNode) {
			if (!int.TryParse(Regex.Replace(img.Name, @"(^\d+).img$", "$1"), out var equipmentId)) {
				Log($"[img名が数字以外]{img.Node.FullPath}");
				return null;
			}
			if (!img.TryExtract()) {
				throw new Exception();
			}
			var info = img.Node.Nodes.SingleOrDefault(x => x.Text == "info");
			var equipment = new Equipment {
				EquipmentId = equipmentId
			};

			Console.CursorLeft = 0;
			Console.Write(img.Name);
			if (info != null) {
				foreach (var node in info.Nodes) {
					switch (node.Text) {
						case "islot":
							equipment.Islot = (string)node.Value;
							break;
						case "vslot":
							equipment.Vslot = (string)node.Value;
							break;
						case "reqJob":
							equipment.ReqJob = GetValue<int>(node.Value);
							break;
						case "reqLevel":
							equipment.ReqLevel = GetValue<int>(node.Value);
							break;
						case "reqSTR":
							equipment.ReqStr = GetValue<int>(node.Value);
							break;
						case "reqDEX":
							equipment.ReqDex = GetValue<int>(node.Value);
							break;
						case "reqINT":
							equipment.ReqInt = GetValue<int>(node.Value);
							break;
						case "reqLUK":
							equipment.ReqLuk = GetValue<int>(node.Value);
							break;
						case "incPDD":
							equipment.IncPdd = GetValue<int>(node.Value);
							break;
						case "incSTR":
							equipment.IncStr = GetValue<int>(node.Value);
							break;
						case "incDEX":
							equipment.IncDex = GetValue<int>(node.Value);
							break;
						case "incINT":
							equipment.IncInt = GetValue<int>(node.Value);
							break;
						case "incLUK":
							equipment.IncLuk = GetValue<int>(node.Value);
							break;
						case "tuc":
							equipment.Tuc = GetValue<int>(node.Value);
							break;
						case "price":
							equipment.Price = GetValue<int>(node.Value);
							break;
						case "cash":
							if (node.Value is int val) {
								equipment.Cash = val;
							} else {
								equipment.Cash = int.Parse((string)node.Value);
							}
							break;
						case "icon":
							if (node.Value is Wz_Png) {
								equipment.Icon = CreateImage(node);
							} else {
								equipment.IconImageLogicalPath = node.ResolveUol().FullPath;
							}
							break;
						case "iconRaw":
							if (node.Value is Wz_Png) {
								equipment.IconRaw = CreateImage(node);
							} else {
								equipment.IconRawImageLogicalPath = node.ResolveUol().FullPath;
							}
							break;
					}
				}
			}

			equipment.Frames = new List<Frame>();

			foreach (var motion in img.Node.Nodes.Where(x => x.Text != "info")) {
				// フレーム番号なし defaultなど
				if (motion.Nodes.Any(x => x.Value is Wz_Png || x.Value is Wz_Uol)) {
					var frame = CreateFrame(equipmentId, motion.Text, "", -1, motion, frameIncludeSubNode);
					equipment.Frames.Add(frame);
				}

				if (!motion.Nodes.Any(x => x.Value is Wz_Png) &&
					motion.Nodes.Any(x => x.Value == null && x.Nodes.Any(n => n.Value is Wz_Png || n.Value is Wz_Uol))) {
					// フレーム番号あり、パターンなし ノーマルパターン
					foreach (var wzFrame in motion.Nodes.Where(x => x.Value == null && x.Nodes.Any(n => n.Value is Wz_Png || n.Value is Wz_Uol))) {
						if (!int.TryParse(wzFrame.Text, out _)) {
							Log($"[フレーム番号が数字ではない]{img.Name} - {motion.Text} - {wzFrame.Text}");
							continue;
						}
						var frame = CreateFrame(equipmentId, motion.Text, "", int.Parse(wzFrame.Text), wzFrame, frameIncludeSubNode);
						equipment.Frames.Add(frame);
					}
				}

				if (!motion.Nodes.Any(x => x.Value is Wz_Png) &&
					!motion.Nodes.Any(x => x.Value == null && x.Nodes.Any(n => n.Value is Wz_Png))) {
					// フレーム番号あり、パターンあり
					foreach (var outer in motion.Nodes.Where(x => x.Value == null)) {
						foreach (var inner in outer.Nodes.Where(x => x.Value == null && x.Nodes.Any(n => n.Value is Wz_Png || n.Value is Wz_Uol))) {
							string frameno;
							string pattern;
							if (motion.Nodes.Any(x => x.Text == "0")) {
								pattern = inner.Text;
								frameno = outer.Text;
							} else {
								pattern = outer.Text;
								frameno = inner.Text;
							}
							if (!int.TryParse(frameno, out _)) {
								Log($"[フレーム番号が数字ではない]{img.Name} - {motion.Text} - {pattern} - {frameno}");
								continue;
							}
							var frame = CreateFrame(equipmentId, motion.Text, pattern, int.Parse(frameno), inner, frameIncludeSubNode);
							equipment.Frames.Add(frame);
						}
					}
				}
			}

			return equipment;
		}

		private static Frame CreateFrame(int imageId, string motionName, string pattern, int frameNo, Wz_Node frameNode, bool includeSubNode) {
			var frame = new Frame {
				Motion = motionName,
				Pattern = pattern,
				FrameNo = frameNo
			};
			frame.FrameImages = CreateFrameImages(frameNode, includeSubNode);

			foreach (var item in frameNode.Nodes) {
				if (item.Value is Wz_Png || item.Value is Wz_Uol uol) {
					continue;
				}
				if (includeSubNode && item.Value == null) {
					continue;
				}
				// body,arm,mail,default
				switch (item.Text) {
					case "face":
						frame.Face = GetValue<int>(item.Value);
						break;
					case "delay":
						frame.Delay = GetValue<int>(item.Value);
						break;
					case "rotate":
						frame.Rotate = GetValue<int>(item.Value);
						break;
					case "flip":
						frame.Flip = GetValue<int>(item.Value);
						break;
					case "hideName":
						frame.HideName = GetValue<int>(item.Value);
						break;
					case "action":
						frame.ReferenceFrameEquipmentId = imageId;
						frame.ReferenceFrameMotion = GetValue<string>(item.Value);
						break;
					case "frame":
						frame.ReferenceFrameFrameNo = GetValue<int>(item.Value);
						break;
					case "move":
						if (item.Value is Wz_Vector moveVector) {
							frame.MoveX = moveVector.X;
							frame.MoveY = moveVector.Y;
						} else {
							Log($"[moveの型不正]{motionName} - {pattern} - {frameNo} - {item.Text}");
						}
						break;
					case "justDir":
						break;
					default:
						Log($"[不明な属性]{motionName} - {pattern} - {frameNo} - {item.Text}");
						break;

				}
			}
			return frame;
		}

		private static FrameImage[] CreateFrameImages(Wz_Node node, bool includeSubNode) {
			var frameImages = new List<FrameImage>();

			foreach (var item in node.Nodes) {
				if (item.Value is Wz_Png) {
					frameImages.Add(new FrameImage() {
						FullPath = item.FullPath,
						Image = CreateImage(item)
					});
				} else if (item.Value is Wz_Uol uol) {
					var resolvedNode = item.ResolveUol();
					if (resolvedNode != null) {
						if (resolvedNode.Value == null) {
							foreach (var itemInResolvedNode in resolvedNode.Nodes) {
								if (itemInResolvedNode.Value is Wz_Png) {
									frameImages.Add(new FrameImage() {
										FullPath = $@"{item.FullPath}\{itemInResolvedNode.FullPath}",
										ImageLogicalPath = itemInResolvedNode.FullPath
									});
								}
							}
						} else {
							frameImages.Add(new FrameImage() {
								FullPath = item.FullPath,
								ImageLogicalPath = resolvedNode.FullPath
							});
						}
					}
				} else if (includeSubNode && item.Value == null) {
					frameImages.AddRange(CreateFrameImages(item, true));
				}
			}
			return frameImages.ToArray();
		}

		/// <summary>
		/// <see cref="Image"/>MapleStoryDB.Image</see>作成/Png画像生成
		/// </summary>
		/// <param name="imageNode">ValueプロパティがWz_PngのWz_Node</param>
		/// <returns>MapleStoryDB.Image</returns>
		private static Image CreateImage(Wz_Node imageNode) {
			if (!(imageNode.Value is Wz_Png png)) {
				throw new Exception("Wz_Pngではない");
			}
			var image = new Image();
			var isLink = false;
			image.ImageActualPath = $"{imageNode.FullPath}";
			image.ImageLogicalPath = imageNode.FullPath;

			foreach (var node in imageNode.Nodes) {
				switch (node.Text) {
					case "origin":
						if (node.Value is Wz_Vector originVector) {
							image.OriginX = originVector.X;
							image.OriginY = originVector.Y;
						} else {
							throw new Exception("originの型不正");
						}
						break;
					case "map":
						image.Maps = new List<Map>();
						foreach (var map in node.Nodes) {
							if (map.Value is Wz_Vector mapVector) {
								image.Maps.Add(new Map() {
									BasedPositionName = map.Text,
									X = mapVector.X,
									Y = mapVector.Y
								});
							} else if (map.Text == "z" || map.Text == "group") {
								// 無視
							} else {
								Log($"[mapがVector型ではない]{imageNode.FullPath} - {map.Text}");
							}
						}
						break;
					case "z":
						if (node.Value is string str) {
							image.Z = str;
						}
						break;
					case "group":
						if (node.Value is string group) {
							image.Group = group;
						}
						break;
					case "_hash":
						if (node.Value is string hash) {
							image.Hash = hash;
						}
						break;
					case "_inlink":
						// default\default -> 01000095.img\defaultdefault
						var imgName = Regex.Replace(imageNode.FullPath, $@"^.*?([^\\].+\.img)\\.+$", "$1");
						image.ImageActualPath = $"{imgName}\\{GetValue<string>(node.Value)}".Replace('/', '\\');
						isLink = true;
						break;
					case "_outlink":
						// Character\Accessory\010120011.img\default\default -> 010120011.img\default\default
						image.ImageActualPath = Regex.Replace(GetValue<string>(node.Value), $@"^.*?/([^/]+\.img.+$)", "$1").Replace('/', '\\');
						isLink = true;
						break;
					case "delay":
					case "face":
						// Image単位で動かすわけではないので、Delayは多分間違って入れた属性
						// faceも同様
						break;
					default:
						if (node.Value is Wz_Png) {
							// Pngの中にPng
							// 間違って入れられている
							break;
						}
						Log($"[不明な属性]{imageNode.FullPath} - {node.Text}");
						break;
				}
			}

			if (!isLink) {
				png.ExtractPng().SaveEx($"{image.ImageActualPath}.png");
			}
			return image;
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
		private static void Log(object text) {
			Console.WriteLine();
			Console.WriteLine(text);
			Console.WriteLine();
		}
	}

	public static class BitmapEx {
		public static void SaveEx(this System.Drawing.Bitmap bmp, string path) {
			// google cloud storageにハングル文字のファイルがアップロードできないので暫定措置
			if (Regex.IsMatch(path, @"[^a-zA-Z0-9\.\\/]")) {
				return;
			}
			var dir = Path.GetDirectoryName(path);
			if (!Directory.Exists(dir)) {
				Directory.CreateDirectory(dir);
			}
			bmp.Save(path);
		}

	}
}
