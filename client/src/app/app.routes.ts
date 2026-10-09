import { Routes } from '@angular/router';
import {
  adminGuard,
  customerGuard,
  salespersonGuard,
  headquartersGuard,
  managementGuard,
  customerGateGuard,
  staffGateGuard,
  sessionGuard,
} from './core/session.guard';
import { Cars } from './features/cars/cars';
import { Categories } from './features/categories/categories';
import { Customers } from './features/customers/customers';
import { Headquarters } from './features/headquarters/headquarters';
import { SignIn } from './features/sign-in/sign-in';
import { Staff } from './features/staff/staff';
import { Performance } from './features/management/performance';
import { ManagementTeam } from './features/management/management';
import { Home } from './features/home/home';
import { Brands } from './features/brands/brands';
import { Products } from './features/products/products';
import { Catalog } from './features/storefront/catalog';
import { Account } from './features/storefront/account';
import { Counter } from './features/storefront/counter';
import { Landing } from './features/storefront/landing';
import { Usados } from './features/storefront/usados';
import { Vehicles } from './features/vehicles/vehicles';
import { Sales } from './features/sales/sales';
import { Shell } from './layout/shell';

export const routes: Routes = [
  { path: '', pathMatch: 'full', component: Landing },
  { path: 'acceso-balder', component: SignIn, canActivate: [staffGateGuard] },
  { path: 'cuenta', component: Account, canActivate: [customerGateGuard] },
  { path: 'catalogo', component: Catalog, canActivate: [customerGuard] },
  { path: 'usados', component: Usados },
  { path: 'counters/:codigo', component: Counter },
  {
    path: '',
    component: Shell,
    canActivate: [sessionGuard],
    children: [
      { path: 'home', component: Home, canActivate: [headquartersGuard] },
      { path: 'headquarters', component: Headquarters, canActivate: [headquartersGuard] },
      { path: 'management', component: ManagementTeam, canActivate: [managementGuard] },
      { path: 'performance', component: Performance, canActivate: [salespersonGuard] },
      { path: 'sales', component: Sales },
      { path: 'cars', component: Cars },
      { path: 'staff', component: Staff, canActivate: [adminGuard] },
      { path: 'products', component: Products },
      { path: 'brands', component: Brands },
      { path: 'categories', component: Categories },
      { path: 'vehicles', component: Vehicles },
      { path: 'customers', component: Customers },
    ],
  },
  { path: '**', redirectTo: '' },
];
