// This file can be replaced during build by using the `fileReplacements` array.
// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

import { IEnvironment } from './IEnvironment';

export const environment: IEnvironment = {
  production: false,
  BaseDataUrl: 'https://localhost:5001/api',
  drinkImgBaseUrl: 'https://bingebuddystorage.blob.core.windows.net/drinkimg/',
  credentials: {
    vapidPublicKey: 'BP7M6mvrmwidRr7II8ewUIRSg8n7_mKAlWagRziRRluXnMc_d_rPUoVWGHb79YexnD0olGIFe_xackYqe1fmoxo',
    googleMapsApiKey: 'AIzaSyCIBT4Pp72R-u_4D_5s3r0IFChgwjpHev8',
  },
};

/*
 * In development mode, to ignore zone related error stack frames such as
 * `zone.run`, `zoneDelegate.invokeTask` for easier debugging, you can
 * import the following file, but please comment it out in production mode
 * because it will have performance impact when throw error
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
