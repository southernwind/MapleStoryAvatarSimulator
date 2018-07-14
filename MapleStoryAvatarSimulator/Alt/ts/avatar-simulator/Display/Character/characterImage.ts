import { Image } from "../../models/image";
import { Map } from "../../models/map";
import { Common,Parts } from "../../common";
import Enumerable from "linq";
import { ItemService } from "../../services/item.service";
import { Smap } from "../../models/smap";
import { Equipment } from "../../models/equipment";
import { Zmap } from "../../models/zmap";

export class CharacterImage {
	
	// 元データ
	equipments: Equipment[] = [];
	// 位置情報付加済みデータ
	parts: Parts[] = [];

	private static smaps: Smap[];

	private static zmaps: Zmap[];
	constructor(private readonly itemService: ItemService) {
		if (CharacterImage.smaps == undefined) {
			this.itemService.getSmaps().subscribe(x => CharacterImage.smaps = x);
		}
		if (CharacterImage.zmaps == undefined) {
			this.itemService.getSmaps().subscribe(x => CharacterImage.zmaps = x);
		}
	}

	update() {
		var visibleImages = new Array<Image>();

		var images = Enumerable.from(this.equipments);
		var reservedVslots = Enumerable.empty<ReservedVslot>();

		// smap順に処理を行う
		// smap上位の部位ほど表示の優先順位が高いため。
		// smap上位の装備とvslotがかぶった場合、下位の装備は表示しない。
		for (var zmap of Enumerable.from(CharacterImage.zmaps).orderByDescending(x=>x.index).toArray()) {

			// 該当zmapのimageを探す
			var image =
				Enumerable
					.from(this.equipments)
					.selectMany(x => Enumerable.from(x.frames![0].images).select(i => {
						i.ownerEquipment = x;
						return i;
					}))
					.firstOrDefault(x=>x.z == zmap.name);

			if (image == null) {
				continue;
			}

			// todo:特殊な耳については後でなんとかする
			if (image.imagePath.endsWith("ear") || image.imagePath.endsWith("lefEar") || image.imagePath.endsWith("highlefEar")) {
				continue;
			}

			// 表示に必要なvslot情報
			// hairだけはsmapのvlots情報を使用する。
			// 他はアイテムのinfo/vslotを使用する。
			var vslots = !zmap.name.startsWith("hair") ? image.ownerEquipment.vslots : Enumerable.from(CharacterImage.smaps).firstOrDefault(x=>x.name == zmap.name).vslots ;

			// 表示に必要なvslotがすでに専有済みの場合、表示しない。
			// ただし、専有済みのvslotだとしても、同一アイテムIDの場合は表示を許可する。
			if (
				Enumerable
					.from(vslots)
					.any(x =>
						reservedVslots
							.where(r => r.equipmentId != image.ownerEquipment.id)
							.select(r => r.vslot).contains(x))) {
				continue;
			}

			// 専有済みのvslotリストを更新する。
			reservedVslots =
				reservedVslots
					.concat(
						Enumerable
							.from(vslots)
							.select(x =>
								new ReservedVslot(x, image.ownerEquipment.id)
							).toArray()
					);

			visibleImages.push(image);
		}

		this.parts = Common.imageListToDisplayPartsList(visibleImages);
	}

}
class ReservedVslot {
	constructor(
		public vslot: string,
		public equipmentId: number) {

	}
}