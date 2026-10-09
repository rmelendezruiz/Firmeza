import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { errorMessage } from '../../core/error-message';
import { staffHome } from '../../core/roles';
import { SessionService } from '../../core/session.service';
import { TerminalButton } from '../../shared/ui/terminal-button';

@Component({
  selector: 'app-sign-in',
  imports: [FormsModule, RouterLink, TerminalButton],
  templateUrl: './sign-in.html',
})
export class SignIn {
  private readonly session = inject(SessionService);
  private readonly router = inject(Router);

  protected email = '';
  protected password = '';
  protected readonly sending = signal(false);
  protected readonly error = signal('');
  protected readonly esCliente = computed(() => this.session.role() === 'Cliente');

  protected cerrarCliente(): void {
    this.session.signOut().subscribe();
  }

  protected send(): void {
    this.error.set('');
    this.sending.set(true);
    this.session.signIn(this.email, this.password).subscribe({
      next: () => {
        this.sending.set(false);
        void this.router.navigateByUrl(staffHome(this.session.role()));
      },
      error: (error: unknown) => {
        this.sending.set(false);
        this.error.set(errorMessage(error));
      },
    });
  }
}
