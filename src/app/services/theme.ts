import { Injectable, signal, effect, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private platformId = inject(PLATFORM_ID);
  isDarkTheme = signal<boolean>(false);

  constructor() {
    // Default to dark theme
    this.isDarkTheme.set(true);

    if (isPlatformBrowser(this.platformId)) {
      // Load theme from local storage
      const savedTheme = localStorage.getItem('theme');
      if (savedTheme) {
        this.isDarkTheme.set(savedTheme === 'dark');
      }
    }

    // Apply theme class to body
    effect(() => {
      if (isPlatformBrowser(this.platformId)) {
        if (this.isDarkTheme()) {
          document.body.classList.add('dark-theme');
          localStorage.setItem('theme', 'dark');
        } else {
          document.body.classList.remove('dark-theme');
          localStorage.setItem('theme', 'light');
        }
      }
    });
  }

  toggleTheme() {
    this.isDarkTheme.update(current => !current);
  }
}
