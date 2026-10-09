import { HttpErrorResponse } from '@angular/common/http';

interface CuerpoError {
  message?: string;
}

export function errorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    const cuerpo = error.error as CuerpoError | null;
    if (cuerpo && typeof cuerpo.message === 'string' && cuerpo.message.length > 0) {
      return cuerpo.message;
    }
    if (error.status === 0) {
      return 'No hay conexión con la API.';
    }
  }
  return 'No se pudo completar la operación.';
}
