import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { errorMessage } from '../../core/error-message';
import { formatMoney } from '../../shared/format';
import { PublishedCar, StorefrontApi } from './storefront-api';

@Component({
  selector: 'app-compra',
  imports: [FormsModule],
  templateUrl: './purchase.html',
  styleUrl: './storefront.css',
})
export class Compra {
  private readonly api = inject(StorefrontApi);

  readonly car = input<PublishedCar | null>(null);
  readonly closed = output<void>();
  readonly hecha = output<string>();

  protected acepta = false;
  protected lleva = false;
  protected metodo = '';
  protected readonly sending = signal(false);
  protected readonly error = signal('');
  protected readonly moneda = formatMoney;

  protected pedir(): void {
    const car = this.car();
    const price = car?.pagePrice;
    if (!car || price === null || price === undefined) {
      this.error.set('Ese carro no tiene precio de página.');
      return;
    }
    if (!this.acepta) {
      this.error.set('Debes aceptar los términos de la compra.');
      return;
    }
    if (car.condition === 'usado' && !this.lleva) {
      this.error.set('Debes confirmar que llevarás el usado al counter en siete días.');
      return;
    }
    if (!this.metodo) {
      this.error.set('Elige transferencia, tarjeta o crédito. El efectivo es solo en sala.');
      return;
    }

    this.sending.set(true);
    this.error.set('');
    this.api.buy(car.id, true, price, this.metodo).subscribe({
      next: (purchase) => {
        this.sending.set(false);
        this.acepta = false;
        this.lleva = false;
        this.metodo = '';
        const deposit =
          purchase.inspectionDeposit === null
            ? `Compra ${purchase.number} de ${purchase.car}. Pago ${purchase.paymentMethod}. Total ${formatMoney(purchase.total)}. Recoge en ${purchase.counter}.`
            : `Compra ${purchase.number} de ${purchase.car} en peritaje. Cargo preliminar ${formatMoney(purchase.inspectionDeposit)}. Llévalo a ${purchase.counter} en 7 días. Peritaje ${purchase.inspectionPhone}. El total sigue siendo ${formatMoney(purchase.total)}.`;
        this.hecha.emit(deposit);
      },
      error: (error: unknown) => {
        this.sending.set(false);
        this.error.set(errorMessage(error));
      },
    });
  }

  protected signOut(): void {
    this.acepta = false;
    this.lleva = false;
    this.metodo = '';
    this.error.set('');
    this.closed.emit();
  }
}
