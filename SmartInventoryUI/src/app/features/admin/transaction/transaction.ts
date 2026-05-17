import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { TransactionService } from '../../../services/transaction.service';
import { Transaction } from '../../../models/transaction.model';
import {DatePipe} from '@angular/common';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-transaction',
  imports: [DatePipe,FormsModule],
  templateUrl: './transaction.html',
  styleUrl: './transaction.css',
})
export class TransactionComponent implements OnInit{

  private transactionService=inject(TransactionService);
  transactions = signal<Transaction[]>([]);
  


  ngOnInit(): void {
   
    this.transactionService.getAllTransactions().subscribe({
    next:(data)=>this.transactions.set(data),
     error:(err)=>console.error(`Failed to fetch data:`,err)
    })
  }

//computed read only the transaction
  todySalesCount=computed(()=>{
    const tody=new Date().toDateString();
    return this.transactions().filter(t=>
      new Date(t.transactionDate).toDateString()===tody&& t.type===1
    ).length;
  })

  totalRestockCount=computed(()=>{
    return this.transactions().filter(t=>
      t.type!== 1 ).length;
  })

   searchQuery=signal<string>('');

   filteredTransactions=computed(()=>{
     const query=this.searchQuery().toLowerCase().trim();
     if(!query){
      return this.transactions();
     }

     return this.transactions().filter(t=>
      t.userID.toString().includes(query) ||
      t.productID.toString().includes(query)
     )
   })

   newTransaction={
    transactionID:null,
    productID:null,
    userID:null,
    quantity:null,
    type:null,
    transactionDate:new Date().toISOString()
   }

   saveTransaction(){
    const dataToSend: any = {
    transactionID: 0, 
    productID: Number(this.newTransaction.productID),
    userID: Number(this.newTransaction.userID),
    quantity: Number(this.newTransaction.quantity),
    type: Number(this.newTransaction.type),
    transactionDate: new Date().toISOString(),
    product: null,
    user: null
  };
    this.transactionService.addTransaction(dataToSend).subscribe({
      next:(res)=>{
        this.transactions.set([...this.transactions(),res]);
        alert('Transaction added successfully!');
     this.onClose();
    },
    error: (err) => console.log("Details of Error 400:", err.error)
    });
   }

   selectedTransaction:Transaction={
    transactionID:0,
    productID:0,
    userID:0,
    quantity:0,
    type:0,
    transactionDate:''
   }


   onShow(transaction:Transaction){
     return this.selectedTransaction={...transaction};
   }

   onDelete(id:any){
    if(confirm("Are you sure do want to delete this transaction?")){
      this.transactionService.deleteTransaction(id).subscribe({
        next:()=>{
          this.transactions.update(privetransaction=>privetransaction.filter(t=>t.transactionID!==id));
       
          alert(`Transaction with id ${id} is deleted successfully .`);
           this.selectedTransaction={
                transactionID:0,
                productID:0,
                userID:0,
                quantity:0,
                type:0,
                transactionDate:''
               }
           const closeBtn = document.querySelector('#showTransactionModal .btn-close') as HTMLElement;
            closeBtn?.click();
          }, 
        error: (err) => console.error('Error during deletion:', err)
      })
    }
   }
   
   onClose(){
          this.newTransaction={
         transactionID:null,
         productID:null,
         userID:null,
         quantity:null,
         type:null,
         transactionDate:''
        }
      const closeBtn = document.querySelector('#addTransactionModal .btn-close') as HTMLElement;
            closeBtn?.click();

   }
}
