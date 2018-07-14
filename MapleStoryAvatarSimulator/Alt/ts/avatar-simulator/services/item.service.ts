import 'reflect-metadata';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';

import { Equipment } from '../models/equipment';
import { ItemCategory } from "../ItemList/itemCategory";
import { Smap } from '../models/smap';
import { Zmap } from '../models/zmap';


@Injectable({
	providedIn: 'root',
})
export class ItemService {
	constructor(private http: HttpClient) { }

	getItemCategories(){
		return this.http.get<ItemCategory[]>(`api/GetItemCategories/`);
	}

	getIcons(rangeBegin: number, rangeEnd: number) {
		return this.http.get<Equipment[]>(`api/GetIcons/${rangeBegin}-${rangeEnd}`);
	}

	getItemImages(motion: string,expression:string, equipmentIdList: number[]) {
		return this.http.get<Equipment[]>(`api/GetItemImages/${motion}/${expression}/?${equipmentIdList.map(x => `eqid=${x}`).join("&")}`);
	}

	getSmaps() {
		return this.http.get<Smap[]>(`api/GetSmaps/`);
	}

	getZmaps() {
		return this.http.get<Zmap[]>(`api/GetZmaps/`);
	}
}