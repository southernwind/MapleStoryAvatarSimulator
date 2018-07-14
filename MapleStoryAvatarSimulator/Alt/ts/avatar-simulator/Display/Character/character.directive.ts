import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
	selector: '[character-host]',
})
export class CharacterDirective {
	constructor(public viewContainerRef: ViewContainerRef) { }
}