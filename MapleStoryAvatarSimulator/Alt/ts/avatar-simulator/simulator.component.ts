import 'reflect-metadata';
import {
	Component,
	OnInit,
	ComponentFactoryResolver,
	ViewContainerRef,
	ViewChild
} from '@angular/core';
import { DisplayComponent } from './Display/display.component';
import { ItemListComponent } from './ItemList/itemList.component';
import { Equipment } from './models/equipment';

@Component({
	selector: 'app-root',
	templateUrl: '../templates/avatar-simulator/simulator.html'
})
export class SimulatorComponent implements OnInit{
	title = 'Angular OK';

	@ViewChild(DisplayComponent) displayHost?: DisplayComponent;
	@ViewChild(ItemListComponent) itemListHost?: ItemListComponent;

	constructor() { }

	ngOnInit() {
	}

	equip(item: Equipment) {
		if (this.displayHost == undefined) {
			return;
		}
		this.displayHost.equip(item);
	}
}
