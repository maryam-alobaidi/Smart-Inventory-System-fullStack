import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {

  private http=inject(HttpClient);

  private  apiUrl='http://localhost:8080/api/Users';

  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.apiUrl);
  }

  addUser(user:User):Observable<User>{
    return this.http.post<User>(this.apiUrl,user);

  }

  deleteUser(id:number):Observable<any>{
   return this.http.delete(`${this.apiUrl}/${id}`);
  }

  editUser(user:User):Observable<any>{
    return this.http.put(`${this.apiUrl}/${user.userID}`,user)
  }
}

