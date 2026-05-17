export interface Transaction {
  transactionID?:number;
  productID: number;  
  productName?:string;    
  userID: number;
  userName?:string;     
  type: number;
  quantity: number;
  transactionDate: string;
}