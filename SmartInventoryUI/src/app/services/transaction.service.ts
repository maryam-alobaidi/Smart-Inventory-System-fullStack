import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Transaction } from '../models/transaction.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TransactionService {

  private http=inject(HttpClient);

  private apiUrl = 'http://localhost:8080/api/Transactions';


  getAllTransactions():Observable<Transaction[]>{
    return this.http.get<Transaction[]>(this.apiUrl);
  }


  addTransaction(transaction:Transaction){
    return this.http.post<Transaction>(this.apiUrl,transaction);
  }

  deleteTransaction(id:Number):Observable<any>{
    return this.http.delete(`${this.apiUrl}/${id}`)

  }

  




}
