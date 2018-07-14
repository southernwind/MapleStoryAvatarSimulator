import { Image } from "./models/image";
import { Map } from "./models/map";
import Enumerable from "linq";

export class Common{
	static imageListToDisplayPartsList(imageList: Image[]):Parts[]{
		let parts = [];
		let images = Enumerable.from(imageList).select(x => ({ image: x, placed: false })).toArray();

		// navelの初期値のみ決定しておく
		let absoluteMaps = [new Map(images[0].image.maps[0].basedPositionName, 0, 0)];

		var beforeCount = 0;
		// 未配置のものが残っている間ループする
		while (
			Enumerable.from(images).count(x => !x.placed) != 0 &&
			beforeCount != Enumerable.from(images).count(x => !x.placed)) {

			beforeCount = Enumerable.from(images).count(x => !x.placed);

			for (let image of images) {
				if (image.placed) {
					// 配置済み
					continue;
				}

				// 配置対象画像の相対座標一覧
				var imageMaps = Enumerable.from(image.image.maps);

				// 配置対象画像と同じ名前の絶対座標を絶対座標一覧から探す
				var baseMap =
					Enumerable.from(absoluteMaps).firstOrDefault(m =>
						imageMaps.select(x => x.basedPositionName)
							.contains(m.basedPositionName));

				// 同じ名前の絶対座標があれば画像配置処理に移る
				if (baseMap != null) {
					// 画像の相対座標
					var imageRelativeMap = imageMaps.first(x => x.basedPositionName == baseMap.basedPositionName);

					// 画像の座標はorigin座標を基点として考える
					// origin座標は画像の左上から右に{originX}px、下に{originY}pxの座標
					var positionX = baseMap.x - (imageRelativeMap.x + image.image.originX);
					var positionY = baseMap.y - (imageRelativeMap.y + image.image.originY);

					absoluteMaps = absoluteMaps.concat(
						imageMaps
							.where(x => Enumerable.from(absoluteMaps).all(m => x.basedPositionName != m.basedPositionName))
							.select(x =>
								new Map(
									x.basedPositionName,
									x.x + positionX + image.image.originX,
									x.y + positionY + image.image.originY))
							.toArray());

					parts.push(new Parts(image.image.imagePath, positionX, positionY, image.image.zIndex, true));
					image.placed = true;
				}
			}
		}

		let minx = Enumerable.from(parts).min(x => x.x);
		let miny = Enumerable.from(parts).max(x => x.y);

		for (let key in parts) {
			parts[key].x -= minx;
			parts[key].y -= miny;
		}

		return parts;
	}
}


export class Parts {
	constructor(
		public imagePath: string,
		public x: number,
		public y: number,
		public z: number,
		public display: boolean
	) { }
}