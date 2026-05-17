using Inventory.DTOs.Model;

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.DataAccess
{
    public static class clsUsersData
    {

        static string connectionString = clsDataAccessSettings.ConnectionString;
        public static async Task<int?> AddNewUsers(string Username, string Password, string Role)
        {
            using (SqlCommand command = new SqlCommand("Sp_AddNewUsers"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Username", Username);
                command.Parameters.AddWithValue("@Password", Password);
                command.Parameters.AddWithValue("@Role", Role);
                command.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output });
                string ID = "@ID";

                return await clsPrimaryFunctions.AddAsync(command, connectionString, ID);
            }
        }

        public static async Task<bool> DeleteUsers(int UserID)
        {
            using (SqlCommand command = new SqlCommand("Sp_DeleteUsers"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", UserID);
                return await clsPrimaryFunctions.DeleteAsync(command,connectionString);
            }
        }

        public static async Task<bool?> UpdateUsers(int UserID, string Username, string Password, string Role)
        {
            using (SqlCommand command = new SqlCommand("Sp_UpdateUsers"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserID", UserID);
                command.Parameters.AddWithValue("@Username", Username);
                command.Parameters.AddWithValue("@Password", Password);
                command.Parameters.AddWithValue("@Role", Role);

                return await clsPrimaryFunctions.UpdateAsync(command, connectionString);
            }
        }

        public static async Task<List<UserModel?>> GetAllUsers()
        {
            using (SqlCommand command = new SqlCommand("Sp_GetAllUsers"))
            {
                List<UserModel?> users = new List<UserModel?>();
                command.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = await clsPrimaryFunctions.GetAsync(command, connectionString);

                while (await reader.ReadAsync())
                {
                    users.Add(new UserModel
                    {
                        UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        Role = reader.GetString(reader.GetOrdinal("Role"))
                    });
                  
                }
                return users;
            }
         
        }

        public async static Task<UserModel?> FindByID(int UserID)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("Sp_GetUsersByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", UserID);
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new UserModel
                                {
                                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                    Username = reader.GetString(reader.GetOrdinal("Username")),
                                    Password = reader.GetString(reader.GetOrdinal("Password")),
                                    Role = reader.GetString(reader.GetOrdinal("Role"))
                                };

                            }
                            else
                            {
                                return null; // No user found with the given ID
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        clsPrimaryFunctions.EntireInfoToEventLoge(ex.Message);
                    }
                }
            }
            return null;
        }

        public async static Task<UserModel?> FindByUsernameAndPassword(string username, string password)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("Sp_GetUserByUsernameAndPassword", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new UserModel
                                {
                                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                    Username = reader.GetString(reader.GetOrdinal("Username")),
                                    Password = reader.GetString(reader.GetOrdinal("Password")),
                                    Role = reader.GetString(reader.GetOrdinal("Role"))
                                };

                            }
                            else
                            {
                                return null; // No user found with the given Username and Password
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        clsPrimaryFunctions.EntireInfoToEventLoge(ex.Message);
                    }
                }
            }
            return null;
        }
    }
    }