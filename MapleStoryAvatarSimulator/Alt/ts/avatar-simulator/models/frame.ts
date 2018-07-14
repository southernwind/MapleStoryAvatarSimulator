import { Image } from "./image";

export class Frame {
	constructor(
		public frameNo: number,
		public images: Image[]
	) {
	}
}