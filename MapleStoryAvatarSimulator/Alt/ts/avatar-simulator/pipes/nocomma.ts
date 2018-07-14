import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
	name: 'noComma'
})
export class NoCommaPipe implements PipeTransform {

	transform(val: number): string {
		if (val !== undefined && val !== null) {
			return val.toString().replace(/,/g, "");
		} else {
			return "";
		}
	}
}