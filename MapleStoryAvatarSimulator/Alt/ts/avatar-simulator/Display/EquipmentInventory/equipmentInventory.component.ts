import { Component, Input, OnInit } from '@angular/core';
import 'reflect-metadata';
import { Character } from '../Character/character';

@Component({
	selector: 'app-equipmentInventory',
	templateUrl: '../templates/avatar-simulator/equipmentInventory.html',

})
export class EquipmentInventoryComponent implements OnInit {

	@Input() character?: Character;

	slotList = [
		{ name: "face", displayName: "FACE" },
		{ name: "cap", displayName: "CAP" },
		{ name: "hair", displayName: "HAIR" },
		{ name: "foreHead", displayName: "FORE HEAD" },
		{ name: "eyeAccessory", displayName: "EYE ACC" },
		{ name: "earAccessory", displayName: "EAR ACC" },
		{ name: "weapon", displayName: "WEAPON" },
		{ name: "clothes", displayName: "CLOTHES" },
		{ name: "subWeapon", displayName: "SUB WEAPON" },
		{ name: "pants", displayName: "PANTS" },
		{ name: "gloves", displayName: "GLOVES" },
		{ name: "cape", displayName: "CAPE" },
		{ name: "shoes", displayName: "SHOES" }
	];

	constructor() { }

	ngOnInit(): void {
	}

}
