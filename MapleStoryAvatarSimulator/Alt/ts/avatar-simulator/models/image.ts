import { Map } from "./map";
import { Equipment } from "./equipment";

export class Image {
	constructor(
		public imagePath: string,
		public originX: number,
		public originY: number,
		public zIndex: number,
		public z: string,
		public maps: Map[],
		public ownerEquipment: Equipment
	) {
	}
}