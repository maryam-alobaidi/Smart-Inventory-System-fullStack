import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { Product } from '../models/product.model';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private http = inject(HttpClient);

  
  private apiUrl = 'https://localhost:7134/api/Products';

  getAllProducts():Observable<Product[]>{
    return this.http.get<Product[]>(this.apiUrl);
  }
  
   addProduct(product: Product, file: File | null) {
      const formData = new FormData();
      
      formData.append('productName', product.productName);
      formData.append('quantity', product.quantity.toString());
      formData.append('price', product.price.toString());
      formData.append('categoryID', product.categoryID.toString());
      formData.append('minStockLevel', product.minStockLevel.toString());
      formData.append('maxCapacity', product.maxCapacity.toString());

      if (file) {
        formData.append('imageFile', file); 
      }

     return this.http.post<Product>(this.apiUrl, formData);

    }

   editProduct(formData: FormData): Observable<any> {
     const id = formData.get('productID'); 
     return this.http.put(`${this.apiUrl}/${id}`, formData);
   }
  
   deleteProduct(id:Number):Observable<any>{
      return this.http.delete(`${this.apiUrl}/${id}`)
    }
}
