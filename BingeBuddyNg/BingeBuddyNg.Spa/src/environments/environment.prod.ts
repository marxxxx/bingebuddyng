import { IEnvironment } from "./IEnvironment";

export const environment: IEnvironment = {
  production: true,
  BaseDataUrl: 'https://bingebuddyapi.azurewebsites.net/api',
  drinkImgBaseUrl: 'https://bingebuddystorage.blob.core.windows.net/drinkimg/',
  credentials: {
    vapidPublicKey: '#{VapidPublicKey}#',
    googleMapsApiKey: '#{GoogleMapsApiKey}#'
  }
};
