import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { throwError } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  
  private apiUrl = 'https://localhost:7134/api/Users'; 

  isAdmin = signal<boolean>(localStorage.getItem('Role') === 'Admin');

  login(credentials: any) {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
       
        const userRole = response.role || response.Role;
        const userToken = response.token || response.Token;

        localStorage.setItem('token', userToken);
        localStorage.setItem('Role', userRole);

        this.isAdmin.set(userRole === 'Admin');

        if (userRole === 'Admin') {
          this.router.navigate(['/admin']);
        } else {
          throw 'Only Admin can enter the system.';        }
      })
    );
  }

  logout() {
    localStorage.clear();
    this.isAdmin.set(false);
    this.router.navigate(['/login']);
  }
}