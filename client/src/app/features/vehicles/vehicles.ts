import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { errorMessage } from '../../core/error-message';
import { TerminalButton } from '../../shared/ui/terminal-button';
import { Vehicle, VehiclesApi } from './vehicles-api';

@Component({
  selector: 'app-vehicles',
  imports: [FormsModule, TerminalButton],
  templateUrl: './vehicles.html',
})
export class Vehicles implements OnInit {
  private readonly api = inject(VehiclesApi);

  protected readonly vehicles = signal<Vehicle[]>([]);
  protected readonly status = signal<'cargando' | 'listo' | 'error'>('cargando');
  protected readonly notice = signal('');
  protected readonly error = signal('');
  protected readonly sending = signal(false);
  protected readonly editingId = signal<string | null>(null);

  protected brand = '';
  protected line = '';
  protected year = '';

  ngOnInit(): void {
    this.load();
  }

  protected load(): void {
    this.status.set('cargando');
    this.error.set('');
    this.api.list().subscribe({
      next: (items) => {
        this.vehicles.set(items);
        this.status.set('listo');
      },
      error: (error: unknown) => {
        this.error.set(errorMessage(error));
        this.status.set('error');
      },
    });
  }

  protected edit(vehicle: Vehicle): void {
    this.editingId.set(vehicle.id);
    this.brand = vehicle.brand;
    this.line = vehicle.line;
    this.year = String(vehicle.year);
    this.notice.set('');
    this.error.set('');
  }

  protected cancel(): void {
    this.editingId.set(null);
    this.brand = '';
    this.line = '';
    this.year = '';
  }

  protected save(): void {
    this.notice.set('');
    this.error.set('');
    this.sending.set(true);
    const command = {
      brand: this.brand,
      line: this.line,
      year: number(this.year),
    };
    const id = this.editingId();
    const request = id ? this.api.update(id, command) : this.api.create(command);
    request.subscribe({
      next: () => {
        this.sending.set(false);
        this.notice.set(id ? 'vehículo actualizado.' : 'vehículo creado.');
        this.cancel();
        this.load();
      },
      error: (error: unknown) => {
        this.sending.set(false);
        this.error.set(errorMessage(error));
      },
    });
  }

  protected remove(vehicle: Vehicle): void {
    this.notice.set('');
    this.error.set('');
    this.api.remove(vehicle.id).subscribe({
      next: () => {
        this.notice.set('vehículo eliminado.');
        if (this.editingId() === vehicle.id) {
          this.cancel();
        }
        this.load();
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }
}

function number(valor: string): number {
  const parseado = Number(valor.trim());
  return Number.isFinite(parseado) ? parseado : 0;
}
