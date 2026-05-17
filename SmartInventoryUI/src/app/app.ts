import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Admin } from "./features/admin/admin";

import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('SmartInventoryUI');
}
