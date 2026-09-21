import { Pipe, PipeTransform } from '@angular/core';
import { environment } from 'src/environments/environment';

@Pipe({ name: 'apiImage' })
export class ApiImagePipe implements PipeTransform {
  transform(path: string | null): string {
    if (!path) return '';
    if (path.startsWith('http')) return path;
    return `${environment.apiOrigin}${path}`;
  }
}
