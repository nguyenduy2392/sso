import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NgClass } from '@angular/common';
import { ToastService } from './services/toast.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NgClass],
  template: `
    <router-outlet />

    <!-- Toast container -->
    <div class="toast-container position-fixed bottom-0 end-0 p-3" style="z-index:9999">
      @for (toast of toastSvc.toasts(); track toast.id) {
        <div class="toast show align-items-center text-white border-0 mb-2"
             [ngClass]="'bg-' + toast.type" role="alert">
          <div class="d-flex">
            <div class="toast-body">{{ toast.message }}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto"
                    (click)="toastSvc.remove(toast.id)"></button>
          </div>
        </div>
      }
    </div>
  `,
})
export class App {
  constructor(public toastSvc: ToastService) {}
}
