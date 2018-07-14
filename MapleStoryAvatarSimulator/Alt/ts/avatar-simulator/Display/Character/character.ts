import { Equipment } from "../../models/equipment";

export class Character{
	[key: string]: Equipment | undefined;
	body: Equipment | undefined;
	head: Equipment | undefined;
	face: Equipment | undefined;
	cap: Equipment | undefined;
	hair: Equipment | undefined;
	foreHead: Equipment | undefined;
	eyeAccessory: Equipment | undefined;
	earAccessory: Equipment | undefined;
	weapon: Equipment | undefined;
	clothes: Equipment | undefined;
	subWeapon: Equipment | undefined;
	pants: Equipment | undefined;
	gloves: Equipment | undefined;
	cape: Equipment | undefined;
	shoes: Equipment | undefined;
}