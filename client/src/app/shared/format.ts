const moneda = new Intl.NumberFormat('es-CO', {
  minimumFractionDigits: 2,
  maximumFractionDigits: 2,
});

const date = new Intl.DateTimeFormat('es-CO', {
  dateStyle: 'short',
  timeStyle: 'short',
});

export function formatMoney(valor: number): string {
  return moneda.format(valor);
}

export function formatDate(valor: string): string {
  return date.format(new Date(valor));
}
