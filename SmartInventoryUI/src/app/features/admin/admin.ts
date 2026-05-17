import { Component, inject } from '@angular/core';
import { RouterLink, RouterOutlet, RouterLinkActive } from "@angular/router";
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-admin',
  imports: [RouterLink, RouterOutlet, RouterLinkActive],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
})
export class Admin {

  auth = inject(AuthService);

  logout(){
   this.auth.logout();
  }
}
