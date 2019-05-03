import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {

  private readonly LanguageStorageKey = 'bingebuddy:language';
  private readonly IsOnboardedStorageKey = 'bingebuddy:isOnboarded';
  public readonly DefaultLanguage = 'de';

  constructor() { }

  getLanguage(): string {
    const lang = localStorage.getItem(this.LanguageStorageKey);
    return lang || this.DefaultLanguage;
  }

  setLanguage(lang: string): void {
    localStorage.setItem(this.LanguageStorageKey, lang);
  }

  getIsOnboarded(): boolean {
    return localStorage.getItem(this.IsOnboardedStorageKey) != null;
  }

  setIsOnboarded() {
    localStorage.setItem(this.IsOnboardedStorageKey, 'true');
  }
}
