export const STAFF_GATE = '/acceso-balder';

export const ROLE = {
  admin: 'Admin',
  companyManager: 'GerenteFirmeza',
  manager: 'Gerente',
  salesperson: 'Comercial',
  customer: 'Cliente',
} as const;

const HEADQUARTERS = new Set<string>([ROLE.admin, ROLE.companyManager]);
const MANAGEMENT = new Set<string>([ROLE.admin, ROLE.companyManager, ROLE.manager]);
const SELLS = new Set<string>([ROLE.manager, ROLE.salesperson]);
const PUBLISHES = new Set<string>([ROLE.companyManager, ROLE.manager]);
const INSPECTION = new Set<string>([ROLE.companyManager, ROLE.manager, ROLE.salesperson]);

const NAMES: Record<string, string> = {
  [ROLE.admin]: 'Admin',
  [ROLE.companyManager]: 'Gerente de Firmeza',
  [ROLE.manager]: 'Gerente de counter',
  [ROLE.salesperson]: 'Comercial',
  [ROLE.customer]: 'Cliente',
};

export function roleName(role: string): string {
  return NAMES[role] ?? role;
}

export function isAdmin(role: string): boolean {
  return role === ROLE.admin;
}

export function seesHeadquarters(role: string): boolean {
  return HEADQUARTERS.has(role);
}

export function seesManagement(role: string): boolean {
  return MANAGEMENT.has(role);
}

export function seller(role: string): boolean {
  return SELLS.has(role);
}

export function publisher(role: string): boolean {
  return PUBLISHES.has(role);
}

export function handlesInspection(role: string): boolean {
  return INSPECTION.has(role);
}

export function isSalesperson(role: string): boolean {
  return role === ROLE.salesperson;
}

export function hasBranch(role: string): boolean {
  return role === ROLE.manager || role === ROLE.salesperson;
}

export function staffHome(role: string): string {
  if (role === ROLE.companyManager) {
    return '/headquarters';
  }
  if (isAdmin(role)) {
    return '/home';
  }
  if (role === ROLE.manager) {
    return '/management';
  }
  if (isSalesperson(role)) {
    return '/performance';
  }
  return '/sales';
}
