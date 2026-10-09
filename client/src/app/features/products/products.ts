import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { errorMessage } from '../../core/error-message';
import { formatMoney } from '../../shared/format';
import { TerminalButton } from '../../shared/ui/terminal-button';
import { CategoriesApi, Category } from '../categories/categories-api';
import { BrandsApi, Brand } from '../brands/brands-api';
import { Product, ProductsApi } from './products-api';
import { VehiclesApi, Vehicle } from '../vehicles/vehicles-api';

@Component({
  selector: 'app-products',
  imports: [FormsModule, TerminalButton],
  templateUrl: './products.html',
})
export class Products implements OnInit {
  private readonly api = inject(ProductsApi);
  private readonly brandsApi = inject(BrandsApi);
  private readonly categoriesApi = inject(CategoriesApi);
  private readonly vehiclesApi = inject(VehiclesApi);

  protected readonly products = signal<Product[]>([]);
  protected readonly brands = signal<Brand[]>([]);
  protected readonly categories = signal<Category[]>([]);
  protected readonly vehicles = signal<Vehicle[]>([]);
  protected readonly status = signal<'cargando' | 'listo' | 'error'>('cargando');
  protected readonly notice = signal('');
  protected readonly error = signal('');
  protected readonly sending = signal(false);
  protected readonly editingId = signal<string | null>(null);
  protected readonly moneda = formatMoney;

  protected search = '';
  protected filter = '';
  protected sku = '';
  protected name = '';
  protected price = '';
  protected quantity = '';
  protected brandId = '';
  protected categoryId = '';
  protected vehicleIds: string[] = [];

  ngOnInit(): void {
    this.cargarCatalogo();
    this.load();
  }

  protected nombreVehiculo(id: string): string {
    const vehicle = this.vehicles().find((item) => item.id === id);
    return vehicle ? `${vehicle.brand} ${vehicle.line} ${vehicle.year}` : id;
  }

  private cargarCatalogo(): void {
    this.brandsApi.list().subscribe({
      next: (items) => this.brands.set(items),
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
    this.categoriesApi.list().subscribe({
      next: (items) => this.categories.set(items),
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
    this.vehiclesApi.list().subscribe({
      next: (items) => this.vehicles.set(items),
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }

  protected load(): void {
    this.status.set('cargando');
    this.error.set('');
    this.api.list(this.search, this.filter).subscribe({
      next: (items) => {
        this.products.set(items);
        this.status.set('listo');
      },
      error: (error: unknown) => {
        this.error.set(errorMessage(error));
        this.status.set('error');
      },
    });
  }

  protected clearSearch(): void {
    this.search = '';
    this.filter = '';
    this.load();
  }

  protected edit(product: Product): void {
    this.editingId.set(product.id);
    this.sku = product.sku;
    this.name = product.name;
    this.price = String(product.price);
    this.quantity = String(product.quantity);
    this.brandId = product.brandId;
    this.categoryId = product.categoryId;
    this.vehicleIds = [...product.vehicleIds];
    this.notice.set('');
    this.error.set('');
  }

  protected cancel(): void {
    this.editingId.set(null);
    this.sku = '';
    this.name = '';
    this.price = '';
    this.quantity = '';
    this.brandId = '';
    this.categoryId = '';
    this.vehicleIds = [];
  }

  protected save(): void {
    this.notice.set('');
    this.error.set('');
    this.sending.set(true);
    const command = {
      sku: this.sku,
      name: this.name,
      price: number(this.price, 0),
      quantity: number(this.quantity, -1),
      brandId: this.brandId || '00000000-0000-0000-0000-000000000000',
      categoryId: this.categoryId || '00000000-0000-0000-0000-000000000000',
      vehicleIds: this.vehicleIds,
    };
    const id = this.editingId();
    const request = id ? this.api.update(id, command) : this.api.create(command);
    request.subscribe({
      next: () => {
        this.sending.set(false);
        this.notice.set(id ? 'product actualizado.' : 'product creado.');
        this.cancel();
        this.load();
      },
      error: (error: unknown) => {
        this.sending.set(false);
        this.error.set(errorMessage(error));
      },
    });
  }

  protected remove(product: Product): void {
    this.notice.set('');
    this.error.set('');
    this.api.remove(product.id).subscribe({
      next: () => {
        this.notice.set('product eliminado.');
        if (this.editingId() === product.id) {
          this.cancel();
        }
        this.load();
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }
}

function number(valor: string, siFalla: number): number {
  const parseado = Number(valor.trim().replace(',', '.'));
  return Number.isFinite(parseado) ? parseado : siFalla;
}
