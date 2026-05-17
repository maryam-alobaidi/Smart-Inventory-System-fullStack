using Inventory.DataAccess;
using Inventory.DTOs.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.BusinessLogic
{
    public class clsUsers
    {
        public enum enMode { addNew = 0, update = 1 }
        public enMode Mode = enMode.addNew;



        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public clsUsers()
        {
            this.UserID = -1;
            this.Username = "";
            this.Password = "";
            this.Role = "";
            this.Mode = enMode.addNew;
        }

        private clsUsers(int UserID, string Username, string Password, string Role)
        {
            this.UserID = UserID;
            this.Username = Username;
            this.Password = Password;
            this.Role = Role;

            this.Mode = enMode.update;
        }

        private async Task<bool> _AddNewUsers()
        {
            // Call DataAccess Layer
            this.UserID = (int)await clsUsersData.AddNewUsers(this.Username, this.Password, this.Role);
            return (this.UserID != -1);
        }

        public async static Task<bool> Delete(int UserID)
        {
            // Call DataAccess Layer
            return await clsUsersData.DeleteUsers(UserID);
        }

        public async static Task<UserModel?> Find(int UserID)
        {
          

            return await clsUsersData.FindByID(UserID);
           
        }

       public async static Task<UserModel?> FindByUsernameAndPassword(string username,string password)
        {
                       return await clsUsersData.FindByUsernameAndPassword(username, password);
        }
        public async static  Task<List<UserModel?>> GetAllUsers()
        {
            return await clsUsersData.GetAllUsers();
        }

        public async Task<bool> Save()
        {
            switch (Mode)
            {
                case enMode.addNew:
                    Mode = enMode.update;
                    return await _AddNewUsers();
                case enMode.update:
                    return await _UpdateUsers();
            }
            return false;
        }

        private async Task<bool> _UpdateUsers()
        {
            // Call DataAccess Layer
            return await clsUsersData.UpdateUsers(this.UserID, this.Username, this.Password, this.Role) ?? false;
        }

    }

}
