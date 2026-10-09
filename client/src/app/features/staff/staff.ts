import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { errorMessage } from '../../core/error-message';
import { hasBranch, roleName, ROLE } from '../../core/roles';
import { TerminalButton } from '../../shared/ui/terminal-button';
import { Account, StaffApi, Branch } from './staff-api';

@Component({
  selector: 'app-staff',
  imports: [FormsModule, TerminalButton],
  templateUrl: './staff.html',
})
export class Staff implements OnInit {
  private readonly api = inject(StaffApi);

  protected readonly accounts = signal<Account[]>([]);
  protected readonly branches = signal<Branch[]>([]);
  protected readonly status = signal<'cargando' | 'listo' | 'error'>('cargando');
  protected readonly notice = signal('');
  protected readonly error = signal('');
  protected readonly sending = signal(false);
  protected readonly roleName = roleName;
  protected readonly hasBranch = hasBranch;
  protected readonly roles = [ROLE.companyManager, ROLE.manager, ROLE.salesperson];

  protected email = '';
  protected password = '';
  protected role: string = ROLE.salesperson;
  protected branchId = '';
  protected traslado: Record<string, string> = {};

  ngOnInit(): void {
    this.load();
  }

  protected load(): void {
    this.status.set('cargando');
    this.error.set('');
    forkJoin({ accounts: this.api.list(), branches: this.api.branches() }).subscribe({
      next: ({ accounts, branches }) => {
        this.accounts.set(accounts);
        this.branches.set(branches);
        this.traslado = Object.fromEntries(accounts.map((account) => [account.id, account.branchId ?? '']));
        this.status.set('listo');
      },
      error: (error: unknown) => {
        this.error.set(errorMessage(error));
        this.status.set('error');
      },
    });
  }

  protected save(): void {
    this.notice.set('');
    this.error.set('');
    this.sending.set(true);
    const command = {
      email: this.email,
      password: this.password,
      role: this.role,
      branchId: hasBranch(this.role) && this.branchId ? this.branchId : null,
    };
    this.api.create(command).subscribe({
      next: (account) => {
        this.sending.set(false);
        this.notice.set(`Cuenta ${account.email} creada como ${roleName(account.role)}${account.branch ? ` en ${account.branch}` : ''}.`);
        this.email = '';
        this.password = '';
        this.load();
      },
      error: (error: unknown) => {
        this.sending.set(false);
        this.error.set(errorMessage(error));
      },
    });
  }

  protected trasladar(account: Account): void {
    const destino = this.traslado[account.id];
    this.notice.set('');
    this.error.set('');
    if (!destino || destino === account.branchId) {
      this.error.set('Elige un counter distinto al actual.');
      return;
    }
    this.api.assignBranch(account.id, destino).subscribe({
      next: (actualizada) => {
        this.notice.set(`${actualizada.email} quedó en ${actualizada.branch}.`);
        this.load();
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }
}
