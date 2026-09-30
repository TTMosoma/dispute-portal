import { Routes } from '@angular/router';
import { LoginComponent } from './login/login';
import { authGuard } from './auth/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: 'transactions',
    canActivate: [authGuard],
    loadComponent: () => import('./transactions/transactions').then(m => m.TransactionsComponent)
  },
    {
    path: 'disputes/new',
    canActivate: [authGuard],
    loadComponent: () => import('./dispute-new/dispute-new').then(m => m.DisputeNewComponent)
  },
    {
    path: 'disputes',
    canActivate: [authGuard],
    loadComponent: () => import('./disputes/disputes').then(m => m.DisputesComponent)
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
