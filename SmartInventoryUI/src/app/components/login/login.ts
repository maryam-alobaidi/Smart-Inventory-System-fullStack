import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})



export class Login {
  authService = inject(AuthService);
  username = '';
  password = '';
  errorMessage =signal<string>('');

  login() {
    this.errorMessage.set('');
    
    const loginData = {
      username: this.username,
      password: this.password
    };

      this.authService.login(loginData).subscribe({
      next: (resp) => {
        localStorage.setItem('userName',this.username || resp.userName);
        console.log('Success: Redirecting...');
      },
      error: (err) => {
      
       if(err==='Only Admin can enter the system.'){
        this.errorMessage.set(err);
       }else{
         this.errorMessage.set("Incorrect username or password");
       }
      }
    });
  }
}