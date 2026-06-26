import { Injectable, signal } from '@angular/core';

export interface ToastItem {
  id: number;
  message: string;
  type: 'success' | 'danger' | 'warning' | 'info';
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  toasts = signal<ToastItem[]>([]);
  private nextId = 0;

  private add(message: string, type: ToastItem['type'], duration = 4000): void {
    const id = ++this.nextId;
    this.toasts.update(list => [...list, { id, message, type }]);
    setTimeout(() => this.remove(id), duration);
  }

  remove(id: number): void {
    this.toasts.update(list => list.filter(t => t.id !== id));
  }

  success(msg: string) { this.add(msg, 'success'); }
  error(msg: string)   { this.add(msg, 'danger'); }
  warning(msg: string) { this.add(msg, 'warning'); }
  info(msg: string)    { this.add(msg, 'info'); }
}
