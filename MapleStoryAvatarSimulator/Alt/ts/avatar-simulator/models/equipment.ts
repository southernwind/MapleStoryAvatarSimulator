import { Frame } from "./frame";

export class Equipment {
	public frames = new Array<Frame>();
	constructor(
		public id: number,
		public name: string,
		public iconPath: string,
		public category1: string,
		public category2: string,
		public islot: string,
		public vslots: string[],
		public loaded: boolean
	) {
	}
}