import { Routes } from '@angular/router';
import { AuthComponent } from './auth/auth';
import { LayoutComponent } from './layout/layout';
import { inject } from '@angular/core';
import { Router } from '@angular/router';

export const authGuard = () => {
  const router = inject(Router);
  if (typeof localStorage !== 'undefined' && localStorage.getItem('auth_token')) {
    return true;
  }
  // Optional: Allow access if "unregistered" logic is desired, but for now redirect to login
  // per Lab 6 "authorization login/password" requirement for at least one role.
  return router.parseUrl('/login');
};

export const routes: Routes = [
    { path: 'login', component: AuthComponent },
    { path: 'app', component: LayoutComponent, canActivate: [authGuard] },
    { path: '', redirectTo: 'login', pathMatch: 'full' },
];
