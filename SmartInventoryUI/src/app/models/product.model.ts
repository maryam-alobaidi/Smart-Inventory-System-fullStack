export interface Product {
  productID: number;      
  productName: string;    
  quantity: number;
  price: number;
  minStockLevel: number;
  maxCapacity: number;
  imagePath?: string;
  categoryID: number;
  categoryName:string;
}