import { Component, computed, inject, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { isAdmin, isSalesperson, roleName, seller, seesHeadquarters, seesManagement } from '../core/roles';
import { SessionService } from '../core/session.service';
import { TerminalButton } from '../shared/ui/terminal-button';

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, TerminalButton],
  templateUrl: './shell.html',
  styleUrl: './shell.css',
})
export class Shell implements OnInit {
  private readonly session = inject(SessionService);
  private readonly router = inject(Router);

  protected readonly email = this.session.email;
  protected readonly branch = this.session.branch;
  protected readonly role = computed(() => roleName(this.session.role()));
  protected readonly isAdmin = computed(() => isAdmin(this.session.role()));
  protected readonly seesHeadquarters = computed(() => seesHeadquarters(this.session.role()));
  protected readonly seesManagement = computed(() => seesManagement(this.session.role()));
  protected readonly seesPerformance = computed(() => isSalesperson(this.session.role()));
  protected readonly canSell = computed(() => seller(this.session.role()));
  protected readonly managementName = computed(() => (this.session.role() === 'Gerente' ? 'Mi counter' : 'Counters'));

  ngOnInit(): void {
    this.session.loadProfile().subscribe({ error: () => undefined });
  }

  protected signOut(): void {
    this.session.signOut().subscribe(() => {
      void this.router.navigateByUrl('/');
    });
  }
}
