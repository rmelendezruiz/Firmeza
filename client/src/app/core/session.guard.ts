import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { staffHome, isAdmin, isSalesperson, STAFF_GATE, ROLE, seesHeadquarters, seesManagement } from './roles';
import { SessionService } from './session.service';

export const sessionGuard: CanActivateFn = () => {
  const session = inject(SessionService);
  const router = inject(Router);
  if (!session.isActive()) {
    return router.createUrlTree([STAFF_GATE]);
  }
  if (session.role() === ROLE.customer) {
    return router.createUrlTree(['/catalogo']);
  }
  return true;
};

export const customerGuard: CanActivateFn = () => {
  const session = inject(SessionService);
  const router = inject(Router);
  if (!session.isActive()) {
    return router.createUrlTree(['/cuenta']);
  }
  if (session.role() !== ROLE.customer) {
    return router.createUrlTree([staffHome(session.role())]);
  }
  return true;
};

export const staffGateGuard: CanActivateFn = () => {
  const session = inject(SessionService);
  if (session.isActive() && session.role() !== ROLE.customer) {
    return inject(Router).createUrlTree([staffHome(session.role())]);
  }
  return true;
};

export const customerGateGuard: CanActivateFn = () => {
  const session = inject(SessionService);
  const router = inject(Router);
  if (!session.isActive()) {
    return true;
  }
  if (session.role() === ROLE.customer) {
    return router.createUrlTree(['/catalogo']);
  }
  return router.createUrlTree([staffHome(session.role())]);
};

export const headquartersGuard: CanActivateFn = () => permitir(seesHeadquarters);

export const managementGuard: CanActivateFn = () => permitir(seesManagement);

export const salespersonGuard: CanActivateFn = () => permitir(isSalesperson);

export const adminGuard: CanActivateFn = () => permitir(isAdmin);

function permitir(regla: (role: string) => boolean) {
  const session = inject(SessionService);
  if (regla(session.role())) {
    return true;
  }
  return inject(Router).createUrlTree([staffHome(session.role())]);
}
