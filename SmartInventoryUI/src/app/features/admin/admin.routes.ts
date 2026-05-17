import { Routes } from '@angular/router';
import { Admin } from './admin'; 
import { Products } from './products/products'; 
import { UsersComponent } from './users/users';
import { ReportsComponent } from './reports/reports';
import {  TransactionComponent } from './transaction/transaction';
import { Categories } from './categories/categories';

export const ADMIN_ROUTES: Routes = [
    { 
        path: '', 
        component: Admin,//father of all
        children:[
            
             { path: 'products', component: Products },
            { path: 'users', component: UsersComponent },
            { path: 'categories', component: Categories },
            { path: 'transaction', component: TransactionComponent }, 
            { path: 'reports', component: ReportsComponent },   
            
            {//لكي تفتح صفحة المنتجات تلقائياً عند الدخول للأدمن
                path:'',redirectTo:'products',pathMatch:'full'
            }
        ]

    },
    
];