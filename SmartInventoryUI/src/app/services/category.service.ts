import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Category } from '../models/category.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private http=inject(HttpClient);
  private apiUrl=`http://localhost:8080/api/Categories`;

  getAllCategories(){
    return this.http.get<Category[]>(this.apiUrl);
  }

  addCategory(category:Category){
    return this.http.post<Category>(this.apiUrl,category);
  }

  editCategory(category:Category):Observable<any>{
    return this.http.put(`${this.apiUrl}/${category.categoryID}`,category)
  }

    deleteCategory(id:Number):Observable<any>{
    return this.http.delete(`${this.apiUrl}/${id}`)
  }

}
