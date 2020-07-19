export interface IEnvironment {
  production: boolean,
  BaseDataUrl: string,
  drinkImgBaseUrl: string,
  credentials: {
    vapidPublicKey: string,
    googleMapsApiKey: string
  }
}
