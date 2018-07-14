import 'reflect-metadata';
import { Component, Input, OnChanges, SimpleChange } from '@angular/core';
import { Equipment } from '../models/equipment';
import Enumerable from 'linq';
import { Common,Parts } from '../common';

@Component({
	selector: 'app-icon',
	templateUrl: '../templates/avatar-simulator/icon.html',

})
export class IconComponent implements OnChanges {

	@Input() equipment?: Equipment;
	parts: Parts[] = [];

	constructor() {
	}

	ngOnChanges(changes: { [propKey: string]: SimpleChange }) {
		if (this.equipment != undefined) {
			if (this.equipment.iconPath != undefined) {
				this.parts = [new Parts(this.equipment.iconPath,0,0,1,true)];
			} else {
				// アイコンが存在しない場合、フレーム[0]の画像で代替する
				this.parts = Common.imageListToDisplayPartsList(this.equipment.frames[0].images);
			}
		}
	}
}