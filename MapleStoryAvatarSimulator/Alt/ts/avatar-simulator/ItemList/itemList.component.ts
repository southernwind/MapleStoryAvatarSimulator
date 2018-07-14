import 'reflect-metadata';
import {
	Component,
	OnInit,
	ComponentFactoryResolver,
	ViewContainerRef,
	EventEmitter,
	ViewChild,
	Output,
	HostListener
} from '@angular/core';
import { Equipment } from '../models/equipment';
import { ItemCategory } from './itemCategory';
import { ItemService } from '../services/item.service';
import Enumerable from 'linq';

@Component({
	selector: 'app-itemList',
	templateUrl: '../templates/avatar-simulator/itemList.html',

})
export class ItemListComponent implements OnInit {
	selectedItemCategory?: ItemCategory;

	itemList: Equipment[] | undefined;
	itemCategories: ItemCategory[] | undefined;

	@Output() onEquip = new EventEmitter<Equipment>();

	constructor(
		private itemService: ItemService
	) { }

	ngOnInit() {
		this.getItemCategories();
		this.getItemList();
	}

	getItemList(): void {

		if (this.selectedItemCategory == null) {
			return;
		}
		var category = this.selectedItemCategory;
		this.itemService
			.getIcons(category.rangeBegin, category.rangeEnd)
			.subscribe(x => {
				for (let eq of x) {
					// todo サーバ側で付与
					eq.category1 = category.itemCategory1;
					eq.category2 = category.itemCategory2;
				}
				this.itemList = x;
			});
	}

	getItemListNext(): void {
		if (this.selectedItemCategory == null) {
			return;
		}
		if (this.itemList == null) {
			return;
		}
		if (this.itemList.length == this.selectedItemCategory.count) {
			return;
		}
		var category = this.selectedItemCategory;
		this.itemService
			.getIcons(Enumerable.from(this.itemList).max(x => x.id), category.rangeEnd)
			.subscribe(x => {
				for (let eq of x) {
					// todo サーバ側で付与
					eq.category1 = category.itemCategory1;
					eq.category2 = category.itemCategory2;
				}

				if (this.itemList == null) {
					return;
				}
				this.itemList.concat(x);
			});
	}

	getItemCategories(): void {
		this.itemService
			.getItemCategories()
			.subscribe(x => {
				this.itemCategories = x;
				this.selectedItemCategory = x[0];
			});
	}

	changeSelectedItemCategory(itemCategory: ItemCategory) {
		this.selectedItemCategory = itemCategory;
		this.getItemList();
	}

	equip(item: Equipment) {
		this.onEquip.emit(item);
		if (item.category1 == "Character") {
			switch (item.category2) {
				case "Head":
					this.onEquip.emit(new Equipment(item.id - 10000, "", "", "Character", "Body", "", [], false));
					break;
				case "Body":
					this.onEquip.emit(new Equipment(item.id + 10000, "","", "Character", "Head", "", [], false));
					break;
			}
		}
	}
}
