import { Component, computed, inject, OnInit, Signal, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TransactionService } from '../../../services/transaction.service';
import { CurrencyPipe } from '@angular/common';
import { ProductService } from '../../../services/product.service';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import { forkJoin } from 'rxjs';
import {Chart,registerables} from 'chart.js'
import { UserService } from '../../../services/user.service';
Chart.register(...registerables);

@Component({
  selector: 'app-reports',
  imports: [FormsModule,CurrencyPipe],
  templateUrl: './reports.html',
  styleUrl: './reports.css',
})

export class ReportsComponent implements OnInit {
  private transactionService = inject(TransactionService);
  private productService = inject(ProductService);
  private userService=inject(UserService);

  startDate: string = '';
  endDate: string = '';
  chart:any;
  stockChart:any;
  filteredTransactions = signal<any[]>([]);
  allTransactions = signal<any[]>([]);
  allProducts = signal<any[]>([]);
  totalValue= signal<number>(0);
  rate:number=0;
  currentAdmin:string='';

  ngOnInit(): void {
    this.loadData();
    this.calculateStockTurnover(); 
    this.calculateStats();
    this.currentAdmin=localStorage.getItem('userName')|| 'Guest Admin';
    this.calculateDeadStock();

  }

//load all the date 
  loadData() {
  forkJoin({
    products: this.productService.getAllProducts(),
    transactions: this.transactionService.getAllTransactions()
  }).subscribe(({ products, transactions }) => {
    this.allProducts.set(products);
    this.allTransactions.set(transactions);
    this.filteredTransactions.set(transactions);

    this.calculateStats();
    this.calculateStockTurnover();
    this.calculateDeadStock();
    this.updateStockChartDistributionChart();
  });
  }

  calculateStats() {
    const transactions = this.filteredTransactions();
    const products = this.allProducts();

    if (transactions.length === 0 || products.length === 0) {
      this.totalValue.set(0);
      return;
    }

      const total= transactions.reduce((total, t) => {
      const product = products.find(p => p.productID === t.productID);
      
      const q = t.quantity || t.Quantity || 0;
      const p = product ? (product.price || product.Price || 0) : 0;

      return total + (q * p);
    }, 0);

    this.totalValue.set(total);
    this.updateChart();
  }

  applyFilter() {
    if (!this.startDate || !this.endDate) {
      alert('Please select both dates!');
      return;
    }

    const start = new Date(this.startDate);
    const end = new Date(this.endDate);
    end.setHours(23, 59, 59);

      const filtered = this.allTransactions().filter(t => {
      const tDate = new Date(t.transactionDate || t.TransactionDate);
      return tDate <= end && tDate >= start;
    });

    this.filteredTransactions.set(filtered);
    this.calculateStats();

  }

  updateChart(){
  const data = this.filteredTransactions();
  const salesCount = data.filter(t => t.type === 1 || t.Type === 1).length;
  const restockCount = data.filter(t => t.type === 0 || t.Type === 0).length;

  if(this.chart){
    this.chart.destroy();
  }

  this.chart = new Chart('myReportChart', {
    type: 'doughnut',
    data: {
      labels: ['Sales (Out)', 'Restock (In)'], 
      datasets: [{
        label: 'Transaction Count',
        
        data: [salesCount, restockCount], 
        
       
        backgroundColor: ['#357bdc', '#198754'], 
        
        hoverOffset: 4
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { position: 'bottom' }
      }
    }
  });
  }

  updateStockChartDistributionChart(){

    const product=this.allProducts();
    const totalCurrentQuantity=product.reduce((t,p)=>t+(p.quantity||0),0);
    const totalMaxCapacity=product.reduce((t,p)=>t+(p.maxCapacity||0),0);

    const remainingSpace=totalMaxCapacity-totalCurrentQuantity;
  if (this.stockChart) {
    this.stockChart.destroy();
  }

  this.stockChart = new Chart('stockDistributionChart', {
    type: 'pie', 
    data: {
      labels: ['Occupied Space', 'Available Space'],
      datasets: [{
        data: [totalCurrentQuantity, remainingSpace > 0 ? remainingSpace : 0],
        backgroundColor: ['#ffc107', '#b7babd'], 
        hoverBackgroundColor: ['#ffb300', '#8c8e8f']
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { position: 'bottom' },
        tooltip: {
          callbacks: {
            label: (context) => {
              const label = context.label || '';
              const value = context.raw || 0;
              return `${label}: ${value} Units`;
            }
          }
        }
      }
    }
  });
  }

  getLowStockProducts(){
        console.log(this.allProducts());

    return this.allProducts().filter(p=>{
      const limit=p.minStockLevel || 10;
       return p.quantity<=limit;
    });
  }

  exportToPDF(){
   const doc=new jsPDF();
   const data=this.getLowStockProducts();

   //add title for file
   doc.setFontSize(18);
   doc.text('Inventory Reorder Alerts Report', 14, 20);
   doc.setFontSize(11);
   doc.setTextColor(100);
   doc.text(`Generated on: ${new Date().toLocaleDateString() }`,14,30);

   //add data for Table
   const tableRows=data.map(p=>[
     p.productName,
     p.quantity.toString(),
     (p.minStockLevel || 10).toString(),
     'Low Stock'
   ]
   )

   //creat table use autoTable
   autoTable(doc, {
      startY: 35,
      head: [['Product Name', 'Current Stock', 'Min Stock Level', 'Status']],
      body: tableRows,
      theme: 'striped',
      headStyles: { fillColor: [220, 53, 69] }, 
      styles: { halign: 'center' },
    });

    //Download file
     doc.save('Inventory_Alerts_Report.pdf');

  }

  calculateStockTurnover(){
    const totalUintsSold=this.allTransactions().filter(t=>t.type===2 ).reduce
    ((sum,p)=>sum+p.quantity||1,0);

    const currentStockLevel=this.allProducts()
    .reduce((sum, p) => sum + p.quantity, 0);

    this.rate=totalUintsSold/(currentStockLevel || 1);
    return this.rate.toFixed(1);
  }

  getStockStatus() {
    if (this.rate >= 4) {
      return { text: 'Healthy', class: 'bg-success-subtle text-success' };
    } else if (this.rate >= 2) {
      return { text: 'Fair', class: 'bg-warning-subtle text-warning' };
    } else {
      return { text: 'Needs Action', class: 'bg-danger-subtle text-danger' };
    }
  }

  exportTransactionsPdf() {
  const doc = new jsPDF();
  const data = this.allTransactions();


  doc.setFontSize(18);
  doc.text('Detailed Transactions Report', 14, 20);

  
  doc.setFontSize(12);
  doc.setTextColor(50);
  doc.text(`Generated By: ${this.currentAdmin}`, 14, 28);

  doc.setFontSize(10);
  doc.setTextColor(100);
  doc.text(`Generated on: ${new Date().toLocaleDateString()}`, 14, 35);
  

    const tableRows = data.map(t => {
    const product = this.getProductNameByID(t.productID);
    
    return[new Date(t.transactionDate).toLocaleDateString(), 
    t.type === 1 ? 'Sale' : 'Restock',
    product?product.productName:'Unknown Product',
    t.quantity.toString(),
    product?`$`+product.price:'$0.0'
  ]});


  autoTable(doc, {
    startY: 40, 
    head: [['Date', 'Type', 'Product Name', 'Quantity', 'Total Price']],
    body: tableRows, 
    theme: 'grid',
    headStyles: { fillColor: [52, 58, 64] },
    styles: { fontSize: 10, cellPadding: 3 }
  });

  doc.save('Full_Transactions_Report.pdf');
  }

  getProductNameByID(id:number){
    return this.allProducts().find(p=>p.productID===id)|| null;
  }

  calculateDeadStock(){
    const tenDaysAgo=new Date();
    tenDaysAgo.setDate(tenDaysAgo.getDate()-10);

    const deadProducts=this.allProducts().filter(
     product=>{
      const hasResentSale=this.allTransactions().some(t=>{
          const tDate=new Date(t.transactionDate);
          return t.productID===product.productID && t.type===2 && tDate>=tenDaysAgo;
      });
      return !hasResentSale && product.quantity>0;
     });
     return deadProducts.length;
  }

  getDeadProductStatus() {
  const count = this.calculateDeadStock();
  console.log("alt of dead product",count);

  if (count <= 3) {
     return { text: 'Healthy', class: 'bg-success-subtle text-success' };
   } else if (count <= 7) {
     return { text: 'Attention', class: 'bg-warning-subtle text-warning' };
   } else {
     return { text: 'Needs Action!', class: 'bg-danger-subtle text-danger' };
   }
  }

  activeUsersToday = computed(() => {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  
  const transactions = this.allTransactions().filter(t => new Date(t.transactionDate) >= today);
  return new Set(transactions.map(t => t.userID)).size;
  });


  exportFilteredDatePdf(){
    const doc = new jsPDF();
    const data = this.filteredTransactions();


    doc.setFontSize(18);
    doc.text('Detailed Transactions Report', 14, 20);

  
    doc.setFontSize(12);
    doc.setTextColor(50);
    doc.text(`Generated By: ${this.currentAdmin}`, 14, 28);

    doc.setFontSize(12);
    doc.setTextColor(50);
    doc.text(`Generated Date From : ${this.startDate} To ${this.endDate}`, 14, 30);


    doc.setFontSize(10);
    doc.setTextColor(100);
    doc.text(`Generated on: ${new Date().toLocaleDateString()}`, 14, 35);
    

      const tableRows = data.map(t => {
      const product = this.getProductNameByID(t.productID);

      return[new Date(t.transactionDate).toLocaleDateString(), 
      t.type === 1 ? 'Sale' : 'Restock',
      product?product.productName:'Unknown Product',
      t.quantity.toString(),
      product?`$`+product.price:'$0.0'
    ]});


    autoTable(doc, {
      startY: 40, 
      head: [['Date', 'Type', 'Product Name', 'Quantity', 'Total Price']],
      body: tableRows, 
      theme: 'grid',
      headStyles: { fillColor: [52, 58, 64] },
      styles: { fontSize: 10, cellPadding: 3 }
    });

    doc.save('Filtered_Transactions_Report.pdf');
  }

 
}