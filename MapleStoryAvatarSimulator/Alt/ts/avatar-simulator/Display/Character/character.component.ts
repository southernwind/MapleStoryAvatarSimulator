import 'reflect-metadata';
import { Component, OnInit, Input } from '@angular/core';
import { Character } from './character';
import { Equipment } from '../../models/equipment';
import {ItemService} from '../../services/item.service';
import { Image } from '../../models/image';
import Enumerable from 'linq';
import { CharacterImage } from './characterImage';
import { Smap } from '../../models/smap';

@Component({
	selector: 'app-character',
	templateUrl: '../templates/avatar-simulator/character.html',

})
export class CharacterComponent implements OnInit {
	character: Character;
	motion: string;
	expression: string;
	characterImage = new CharacterImage(this.itemService);

	constructor(private readonly itemService: ItemService) {
		this.character = new Character();
		this.motion = "stand1";
		this.expression = "default";
	}

	ngOnInit() {
	}

	// 装備中アイテムの変更を行う
	equip(item: Equipment): void {
		this.setItem(item);

		var items = new Array<number>();

		for (let key in this.character) {
			let cp = this.character[key];
			if (cp != undefined && !cp.loaded) {
				items.push(cp.id);
			}
		}

		this.itemService
			.getItemImages(this.motion, this.expression, items)
			.subscribe(x => {
				for (let loadedItem of x) {
					for (let key in this.character) {
						let cp = this.character[key];
						if (cp != undefined && cp.id == loadedItem.id) {
							// todo サーバ側で付与 そうすれば(内側のfor),ifも不要になり
							// loadedItemを単純にsetItemするだけでよくなる
							loadedItem.category1 = cp.category1;
							loadedItem.category2 = cp.category2;
							this.setItem(loadedItem);
						}
					}
				}

				this.updateImages();
			});
	}

	// 装備アイテムの更新に伴うキャラクターイメージの更新を行う。
	private updateImages() {
		let items = new Array<Equipment>();
		
		for (let key in this.character) {
			let cp = this.character[key];
			if (cp != undefined && cp.loaded) {
				items = items.concat(cp);
			}
		}
		this.characterImage.equipments = items;
		this.characterImage.update();
	}

	// アイテムカテゴリーによって、装備する部位を振り分ける
	private setItem(item: Equipment) {
		switch (item.category1) {
			case 'Character':
				switch (item.category2) {
					case 'Body':
						this.character.body = item;
						break;
					case 'Head':
						this.character.head = item;
						break;
					case 'Hair':
						this.character.hair = item;
						break;
					case 'Face':
						this.character.face = item;
						break;
				}
				break;
			case 'Armor':
				switch (item.category2) {
					case 'Cap':
						this.character.cap = item;
						break;
					case 'Coat':
						this.character.clothes = item;
						break;
					case 'Longcoat':
						this.character.clothes = item;
						if (item != undefined) {
							this.character.pants = undefined;
						}
						break;
					case 'Pants':
						this.character.pants = item;
						if (item != undefined && this.character.clothes != undefined) {
							if (this.character.clothes.category2 == "LongCoat") {
								this.character.clothes = undefined;
							}
						}
						break;
					case 'Shoes':
						this.character.shoes = item;
						break;
					case 'Glove':
						this.character.gloves = item;
						break;
					case 'Shield':
						this.character.subWeapon = item;
						if (item != undefined) {
							if (this.character.weapon != undefined) {
								if (this.character.weapon.category2 == "TwoHandWeapon") {
									this.character.weapon = undefined;
								}
							}
						}
						break;
					case 'Cape':
						this.character.cape = item;
						break;
				}
				break;
			case 'Accessory':
				switch (item.category2) {
					case 'Face':
						this.character.foreHead = item;
						break;
					case 'Eye':
						this.character.eyeAccessory = item;
						break;
					case 'Ear':
						this.character.earAccessory = item;
						break;
				}
				break;
			case 'OneHandWeapon':
				this.character.weapon = item;
				break;
			case 'TwoHandWeapon':
				this.character.weapon = item;
				if (item != undefined) {
					this.character.subWeapon = undefined;
				}
				break;
			case 'SubWeapon':
				this.character.subWeapon = item;
				if (item != undefined) {
					if (this.character.weapon != undefined) {
						if (this.character.weapon.category2 == "TwoHandWeapon") {
							this.character.weapon = undefined;
						}
					}
				}
				break;
		}
	}
}
