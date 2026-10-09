import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { errorMessage } from '../../core/error-message';
import { roleName, ROLE, seesHeadquarters } from '../../core/roles';
import { SessionService } from '../../core/session.service';
import { formatMoney } from '../../shared/format';
import { TerminalButton } from '../../shared/ui/terminal-button';
import { PerformanceRow, ManagementApi, Mensaje, Tablero } from './management-api';

@Component({
  selector: 'app-management',
  imports: [FormsModule, TerminalButton],
  templateUrl: './management.html',
})
export class ManagementTeam implements OnInit {
  private readonly api = inject(ManagementApi);
  private readonly session = inject(SessionService);

  protected readonly board = signal<Tablero | null>(null);
  protected readonly messages = signal<Mensaje[]>([]);
  protected readonly error = signal('');
  protected readonly notice = signal('');
  protected readonly moneda = formatMoney;
  protected readonly roleName = roleName;
  protected readonly veTodos = computed(() => seesHeadquarters(this.session.role()));
  protected readonly conFicha = computed(() => this.cuentasConFicha());
  protected readonly conMeta = computed(() => this.comerciales());
  protected period = periodoActual();
  protected branchId = '';
  protected fichaCorreo = '';
  protected salary = '';
  protected commission = '';
  protected metaCorreo = '';
  protected metaMonto = '';
  protected con = '';
  protected text = '';

  ngOnInit(): void {
    this.load();
  }

  protected load(): void {
    this.error.set('');
    this.api.board(this.period, this.branchId).subscribe({
      next: (board) => {
        this.board.set(board);
        this.branchId = board.branches.length > 0 ? board.branch.id : '';
        if (!this.con && board.contacts.length > 0) {
          this.con = board.contacts[0].email;
          this.read();
        }
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }

  protected saveStaffCard(): void {
    this.notice.set('');
    this.api.saveStaffCard(this.fichaCorreo, number(this.salary), number(this.commission)).subscribe({
      next: () => {
        this.notice.set(`Ficha de ${this.fichaCorreo} guardada.`);
        this.load();
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }

  protected saveTarget(): void {
    this.notice.set('');
    this.api.saveTarget(this.metaCorreo, this.period, number(this.metaMonto)).subscribe({
      next: () => {
        this.notice.set(`Meta de ${this.metaCorreo} para ${this.period} guardada.`);
        this.load();
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }

  protected read(): void {
    if (!this.con) {
      return;
    }
    this.api.messages(this.con).subscribe({
      next: (items) => this.messages.set(items),
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }

  protected send(): void {
    this.api.send(this.con, this.text).subscribe({
      next: () => {
        this.text = '';
        this.read();
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }

  protected lider(row: PerformanceRow, index: number): string {
    if (index === 0 && row.amount > 0) {
      return 'Más sales';
    }
    if (row.target > 0 && row.progress >= 100) {
      return 'Meta cumplida';
    }
    if (row.target > 0) {
      return 'En camino';
    }
    return 'Sin meta';
  }

  private comerciales(): string[] {
    const datos = this.board();
    if (!datos) {
      return [];
    }
    const roles = new Map(datos.contacts.map((contacto) => [contacto.email, contacto.role]));
    return datos.rows.map((row) => row.email).filter((email) => roles.get(email) === ROLE.salesperson);
  }

  private cuentasConFicha(): string[] {
    const datos = this.board();
    const items = this.comerciales();
    if (datos?.manager && this.veTodos()) {
      items.unshift(datos.manager);
    }
    return items;
  }
}

function periodoActual(): string {
  const hoy = new Date();
  return `${hoy.getFullYear()}-${String(hoy.getMonth() + 1).padStart(2, '0')}`;
}

function number(valor: string): number {
  const parseado = Number(valor.trim().replace(',', '.'));
  return Number.isFinite(parseado) ? parseado : 0;
}
