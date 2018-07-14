import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { SimulatorModule } from './avatar-simulator/simulator.module';
import 'zone.js';

platformBrowserDynamic().bootstrapModule(SimulatorModule)
	.catch(err => console.log(err));