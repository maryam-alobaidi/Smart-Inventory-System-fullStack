import { Component, ElementRef, inject, OnInit, signal, ViewChild } from '@angular/core';
import { ProductService } from '../../../services/product.service';
import { Product } from '../../../models/product.model';
import { NgClass,CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core'; 
import { FormsModule,FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Category } from '../../../models/category.model';
import { CategoryService } from '../../../services/category.service';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-products',
  imports: [NgClass,CommonModule,FormsModule,ReactiveFormsModule],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class Products implements OnInit {




  productService=inject(ProductService);
  categoryService=inject(CategoryService);
  fb=inject(FormBuilder);
  cdr=inject(ChangeDetectorRef);

  products=signal<Product[]>([]);
  categories=signal<Category[]>([]);
  @ViewChild('fileInput') fileInputVariable!: ElementRef;

  ngOnInit():void{
   this.loadProducts();
   this.loadCategories();
  }

  loadProducts(){
      return this.productService.getAllProducts().subscribe({
        next:(data)=>{
         this.products.set([...data]);   
      },
      error: (err) => {
        console.error('Error', err);
      }
      })
  }

  

  productForm = this.fb.group({
          productName: this.fb.control<string>('', { 
            nonNullable: true, 
            validators: [Validators.required, Validators.minLength(3)] 
          }),
          
          quantity: this.fb.control<number>(1, { 
            nonNullable: true, 
            validators: [Validators.required, Validators.min(1)] 
          }),
          
          price: this.fb.control<number>(0, { 
            nonNullable: true, 
            validators: [Validators.required, Validators.min(0)] 
          }),
          
          minStockLevel: this.fb.control<number>(1, { 
            nonNullable: true, 
            validators: [Validators.required, Validators.min(1)] 
          }),
          
          maxCapacity: this.fb.control<number>(1, { 
            nonNullable: true, 
            validators: [Validators.required, Validators.min(1)] 
          }),
          
          categoryID: this.fb.control<number>(0, { 
            nonNullable: true, 
            validators: [Validators.required, Validators.min(1)] 
          }),
         
          imagePath: this.fb.control<string>('', { nonNullable: true })
  });

  saveAdd(){
    
  if (this.productForm.invalid) {
    this.productForm.markAllAsTouched();
    return;
  }

  const productData = this.productForm.getRawValue();

  if (productData.quantity > productData.maxCapacity) {
    alert("Warning: Quantity is more than Max Capacity!");
    return;
  }

      this.productService.addProduct(productData as any, this.selectedFile).subscribe({  
      next: (createdProduct) => {
      const selectedCat = this.categories().find(c => c.categoryID == createdProduct.categoryID);
      const productWithCat = {
        ...createdProduct,
        categoryName: selectedCat ? selectedCat.categoryName : ''
      };

      this.products.update(all => [...all, productWithCat]);
      this.closeModel('addProductModal');
      this.resetNewProduct();
      
      alert('Product Added successfully. 🎉');
          },
          error: (err) => {
            console.error('Failed during Add:', err);
            alert('Added failed on server');
          }
        });
  }

      
  closeModel(modalID:string){
    const modalElement=document.getElementById(modalID);

    const closeBtn = modalElement?.querySelector<HTMLElement>('[data-bs-dismiss="modal"]');
    if (closeBtn) {
    closeBtn.click();
  }
  }


 loadCategories() {
    this.categoryService.getAllCategories().subscribe({
      next: (data) => this.categories.set(data),
      error: (err) => console.error('Error loading categories', err)
    });
  }


  imagePreview:string|null=null;
  selectedFile:File|null=null;


  onFileSelected(event: any) {
  const file = event.target.files[0];
  if (file) {
    this.selectedFile = file; 
    this.imagePreview = URL.createObjectURL(file);
  }
  }

  handleImageError(event: any) {
  const fallback = 'assets/no-image.jpg';
  
  if (event.target.src.includes(fallback)) {
    event.target.src = 'data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7';
    return;
  }
  
  event.target.src = fallback;
  }

  productEditForm=this.fb.group({
    productID:[0],
    productName:['',[Validators.required,Validators.minLength(3)]],
    quantity:[1,[Validators.required,Validators.min(1)]],
    price:[0,[Validators.required,Validators.min(0)]],
    maxCapacity: [1, [Validators.required, Validators.min(1)]],
    categoryID: [0, [Validators.required, Validators.min(1)]]
  })

  onEdit(product:Product){

    this.productEditForm.patchValue({
    productID: product.productID,
    productName: product.productName,
    quantity: product.quantity,
    price: product.price,
    maxCapacity: product.maxCapacity,
    categoryID: product.categoryID
    });
    this.imagePreview=product.imagePath? 'https://localhost:7134/' + product.imagePath : null;
 }

  onDelete(id:number){

     if(confirm("Are you sure do want to delete this product?")){
      this.productService.deleteProduct(id).subscribe({
        next:()=>{
          this.products.update(priveProduct=>priveProduct.filter(p=>p.productID!==id));
          alert(`Product with id ${id} is not active now.`);
        }, 
        error: (err) => console.error('Error during deletion:', err)
      })
    }
  }

  saveEdit() {
   
    if(this.productEditForm.invalid){
      console.warn('Form is invalid! Check errors:', this.productEditForm.errors);
      this.productEditForm.markAllAsTouched();
      return;
    }

    const productUpdateData=this.productEditForm.getRawValue();
    const formData = new FormData();

    //for we up the data with an image we use the FromDate

    Object.keys(productUpdateData).forEach(key=>{
      formData.append(key,(productUpdateData as any)[key])});
  
  
    if (this.selectedFile) {
    formData.append('imageFile', this.selectedFile);
    }

    this.productService.editProduct(formData).subscribe({
    next: (response) => {

      const selectedCat = this.categories().find(c => c.categoryID === Number(productUpdateData.categoryID));

      this.products.update(allProducts => 
        allProducts.map(p => 
          p.productID ===Number(productUpdateData.productID)
          ? { 
              ...p, ...(productUpdateData as any), 
              imagePath: response.imagePath || p.imagePath, 
              categoryName: selectedCat ? selectedCat.categoryName : p.categoryName
            } 
          : p
        )
      );
      
      this.closeModel('editProductModal');
      alert('Data updated successfully. 🎉');
      
      this.selectedFile = null;
      if (this.imagePreview) URL.revokeObjectURL(this.imagePreview);
      this.imagePreview = null;
    },
    error: (err) => {
      console.error('Failed during Edit:', err);
      alert('Data update failed on server');
    }
  });
}


 resetNewProduct() {
  if (this.imagePreview) {
    URL.revokeObjectURL(this.imagePreview); // تنظيف الرابط من الذاكرة
  }
  this.productForm.reset({ });
  this.imagePreview = null;
  this.selectedFile = null;
}
}
  

