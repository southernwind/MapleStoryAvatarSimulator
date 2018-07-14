import 'reflect-metadata';
import {
	Component,
	OnInit,
	ComponentFactoryResolver,
	ViewContainerRef,
	ViewChild
} from '@angular/core';
import { Equipment } from '../models/equipment';
import { CharacterDirective } from './Character/character.directive';
import { CharacterComponent } from './Character/character.component';

@Component({
	selector: 'app-display',
	templateUrl: '../templates/avatar-simulator/display.html',

})
export class DisplayComponent implements OnInit {
	title = 'Angular OK';
	selectedCharacter: CharacterComponent | undefined;

	@ViewChild(CharacterDirective) characterHost?: CharacterDirective;

	constructor(
		private componentFactoryResolver: ComponentFactoryResolver
	) { }

	ngOnInit() {
		if (this.characterHost == null) {
			return;
		}

		this.selectedCharacter =
			this.characterHost
				.viewContainerRef
				.createComponent(
					this.componentFactoryResolver
						.resolveComponentFactory(CharacterComponent)
				).instance;
	}

	equip(item: Equipment): void {
		if (this.selectedCharacter == undefined) {
			return;
		}
		this.selectedCharacter.equip(item);
	}
}
