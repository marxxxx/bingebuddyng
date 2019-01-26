import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {

  private readonly StorageKey = 'bingebuddy:language';
  public readonly DefaultLanguage = 'de';

  constructor() { }

  getLanguage(): string {
    const lang = localStorage.getItem(this.StorageKey);
    return lang || this.DefaultLanguage;
  }

  setLanguage(lang: string): void {
    localStorage.setItem(this.StorageKey, lang);
  }
}
