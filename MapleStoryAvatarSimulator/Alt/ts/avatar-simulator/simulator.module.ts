import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http'; 
import { NgModule } from '@angular/core';

import { NoCommaPipe } from './pipes/noComma';
import { DisplayComponent } from './Display/display.component';
import { ItemListComponent } from './ItemList/itemList.component';
import { CharacterDirective } from './Display/Character/character.directive';
import { SimulatorComponent } from './simulator.component';
import { CharacterComponent } from './Display/Character/character.component';
import { EquipmentInventoryComponent } from './Display/EquipmentInventory/equipmentInventory.component';
import { IconComponent } from './icon/icon.component';

@NgModule({
	declarations: [
		SimulatorComponent,
		DisplayComponent,
		CharacterComponent,
		ItemListComponent,
		NoCommaPipe,
		CharacterDirective,
		EquipmentInventoryComponent,
		IconComponent
	],
	imports: [
		BrowserModule,
		HttpClientModule
	],
	providers: [],
	bootstrap: [SimulatorComponent],
	entryComponents: [CharacterComponent]
})
export class SimulatorModule { }