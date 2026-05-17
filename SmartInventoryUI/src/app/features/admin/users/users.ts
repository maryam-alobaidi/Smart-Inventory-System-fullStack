import { Component, inject, model, OnInit, signal } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { User } from '../../../models/user.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-users',
  imports: [CommonModule,FormsModule],
  templateUrl: './users.html',
  styleUrl: './users.css',
})
export class UsersComponent implements OnInit {
  [x: string]: any;
    private userService=inject(UserService);
    users = signal<User[]>([]);

  ngOnInit(): void {
    this.userService.getAllUsers().subscribe({
    next:(data)=>this.users.set(data),
    error:(err)=>console.error(`Failed to fetch data:`,err)
   })
  }


  selectedUser:User={userID:0,username:'',password:'',role:''};

  onEdit(user:User){
     this.selectedUser={...user};

  }

  saveEdit(){

    this.userService.editUser(this.selectedUser).subscribe({
      next:()=>{
      this.users.update(allUsers => 
        allUsers.map(u => u.userID === this.selectedUser.userID ? { ...this.selectedUser } : u)
      );
      this.closeModal('EditUserModal');
    alert('Data update successfully.');  
    },
     error: (err) => {
      console.error('Failed durring Edit:', err);
      alert('Data update failed on server');    }

    })
  }

  onDelete(id:number){
    if(confirm("Are you sure do want to delete this user?")){
      this.userService.deleteUser(id).subscribe({
        next:()=>{
        this.users.update(prevUsers => prevUsers.filter(u => u.userID !== id));
        alert(`User with id ${id} deleted successfully.`);
        },
        
        error: (err) => console.error('Error during deletion:', err)
        
      });
    }
  }


  newUser:User={
    userID:0,username:'',password:'',role:'User'
  }

  saveAdd(){
  
    this.userService.addUser(this.newUser).subscribe({
      next:(createdUser)=>{
        this.users.update(allUsers=>[...allUsers,createdUser]);
        this.newUser = { userID: 0, username: '', password: '', role: 'User' };
        this.closeModal('addUserModal');
        alert('User Added successfully.');
      },

      error:(err)=>{
          console.error('Failed durring Add:', err);
          alert('Added failed on server');  
      }
    })
  }


  closeModal(modalID:string) {
    const modalElement = document.getElementById(modalID);

    const closeBtn = modalElement?.querySelector<HTMLElement>('[data-bs-dismiss="modal"]');
    if (closeBtn) {
    closeBtn.click();
  }
  }

}
