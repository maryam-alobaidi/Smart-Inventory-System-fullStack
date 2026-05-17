import { Component, inject, OnInit, signal, Signal } from '@angular/core';
import { CategoryService } from '../../../services/category.service';
import { Category } from '../../../models/category.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-categories',
  imports: [CommonModule,FormsModule],
  templateUrl: './categories.html',
  styleUrl: './categories.css',
})
export class Categories implements OnInit {

    private categoryService=inject(CategoryService);
    categories=signal<Category[]>([]);


  ngOnInit(): void {
   this.categoryService.getAllCategories().subscribe({
    next:(data)=>{
      this.categories.set(data)
    },
    error:(err)=>{
      console.error('ailed to fetch data:',err);
    }
   })

  }

  selectedCategory:Category={categoryID:0,categoryName:''}

  onEdit(category:Category){
   this.selectedCategory={...category};
  }

  saveEdit(){
    this.categoryService.editCategory(this.selectedCategory).subscribe({
       next:()=>{ 
        this.categories.update(allCategories=>allCategories.map(c=>c.categoryID===this.selectedCategory.categoryID?this.selectedCategory:c));
        this.closeModel('editCategoryModal');
        alert('Data update successfully.');
      },
      error:(err)=>{
        console.error('Failed durring Edit:', err);
      alert('Data update failed on server');    }
    
    })
  }

  onDelete(id:Number){
    if(confirm("Are you sure do want to delete this category?")){
      this.categoryService.deleteCategory(id).subscribe({
        next:()=>{
          this.categories.update(priveCategory=>priveCategory.filter(c=>c.categoryID!==id));
          alert(`Category with id ${id} deleted successfully.`);
        }, 
        error: (err) => console.error('Error during deletion:', err)
      })
    }
  }

  newCategory:Category={categoryID:0,categoryName:''}

  saveAdd(){
   this.categoryService.addCategory(this.newCategory).subscribe({
    next:(createdCategory)=>{
    this.categories.update(allCategories=>[...allCategories,createdCategory]);
    this.closeModel('addCategoryModal');
    this.newCategory={categoryID:0,categoryName:''};
    alert('User Added successfully.');
    },
     error:(err)=>{
          console.error('Failed durring Add:', err);
          alert('Added failed on server');  
      }
   })

  }

  closeModel(modalID:string){
    const modalElement=document.getElementById(modalID);

    const closeBtn = modalElement?.querySelector<HTMLElement>('[data-bs-dismiss="modal"]');
    if (closeBtn) {
    closeBtn.click();
  }
  }

}
