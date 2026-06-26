import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgClass } from '@angular/common';
import { AuthService } from '../../../services/auth.service';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, NgClass],
  templateUrl: './login.html',
})
export class Login implements OnInit {
  form!: FormGroup;
  submitted = false;
  loading = false;
  showPassword = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService,
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      tenantName: ['', Validators.required],
      userName: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  get f() {
    return this.form.controls;
  }

  login(): void {
    this.submitted = true;
    if (this.form.invalid) return;

    this.loading = true;

    this.authService.login(this.form.value).subscribe({
      next: () => {
        const returnUrl = this.route.snapshot.queryParams['returnUrl'];
        if (returnUrl) {
          window.location.href = decodeURIComponent(returnUrl);
        } else {
          this.router.navigateByUrl('/');
        }
      },
      error: (err) => {
        const msg = err?.error?.message ?? err?.message ?? 'Đăng nhập thất bại';
        this.toast.error(msg);
        this.loading = false;
      },
    });
  }
}
