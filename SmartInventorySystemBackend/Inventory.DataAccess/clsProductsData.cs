using Inventory.DTOs.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.DataAccess
{

    public static class clsProductsData
    {
        static string connectionString = clsDataAccessSettings.ConnectionString;
        public static async Task<int?> AddNewProducts(string ProductName, int Quantity, decimal Price, int MinStockLevel,int MaxCapacity, string ImagePath, int CategoryID)
        {
            using (SqlCommand command = new SqlCommand("Sp_AddNewProducts"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductName", ProductName);
                command.Parameters.AddWithValue("@Quantity", Quantity);
                command.Parameters.Add("@Price", SqlDbType.Decimal).Value = Price;
                command.Parameters.AddWithValue("@MinStockLevel", MinStockLevel);
                command.Parameters.AddWithValue("@MaxCapacity", MaxCapacity);
                command.Parameters.AddWithValue("@ImagePath", ImagePath);

                command.Parameters.AddWithValue("@CategoryID", CategoryID);
                command.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;
                string ID = "@ID";

                return await clsPrimaryFunctions.AddAsync(command, connectionString, ID);
            }
        }

        public static async Task<bool> DeleteProducts(int ProductID)
        {
            using (SqlCommand command = new SqlCommand("Sp_DeleteProducts"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", ProductID);
                return await clsPrimaryFunctions.DeleteAsync(command, connectionString);
            }
        }

        public static async Task<List<ProductModel>> GetAllProducts()
        {
            using (SqlCommand command = new SqlCommand("Sp_GetAllProducts"))
            {
                List<ProductModel> products = new List<ProductModel>();
                command.CommandType = CommandType.StoredProcedure;
                using (var reader = await clsPrimaryFunctions.GetAsync(command, connectionString))
                {
                    while (await reader.ReadAsync())
                    {
                        products.Add(new ProductModel
                        {
                            ProductID = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            Quantity = reader.GetInt32(2),
                            Price = reader.GetDecimal(3),
                            MinStockLevel = reader.GetInt32(4),
                            MaxCapacity = reader.GetInt32(5),
                            ImagePath =reader.GetString(6),
                            CategoryID = reader.GetInt32(7),
                            CategoryName = reader.GetString(8)



                        });
                    }
                }
                return products;
            }
        }

        public static async Task<ProductModel?> FindById(int productId)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("Sp_GetProductsByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", productId);

                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ProductModel
                            {
                                ProductID = (int)reader["ProductID"],
                                ProductName = (string)reader["ProductName"],
                                Quantity = (int)reader["Quantity"],
                                Price = (decimal)reader["Price"],
                                MinStockLevel = (int)reader["MinStockLevel"],
                                MaxCapacity = (int)reader["MaxCapacity"],
                                ImagePath = (string)reader["ImagePath"],
                                CategoryID = (int)reader["CategoryID"]
                            };
                        }
                    }
                }
            }
            return null; 
        }
        public static async Task<bool?> UpdateProducts(int ProductID, string ProductName, int Quantity, decimal Price, int MinStockLevel,int MaxCapacity,string ImagePath, int CategoryID)
        {
            using (SqlCommand command = new SqlCommand("Sp_UpdateProducts"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductID", ProductID);
                command.Parameters.AddWithValue("@ProductName", ProductName);
                command.Parameters.AddWithValue("@Quantity", Quantity);
                command.Parameters.AddWithValue("@Price", Price);
                command.Parameters.AddWithValue("@MinStockLevel", MinStockLevel);
                command.Parameters.AddWithValue("@MaxCapacity", MaxCapacity);
                command.Parameters.AddWithValue("@ImagePath", ImagePath);
                command.Parameters.AddWithValue("@CategoryID", CategoryID);

                return await clsPrimaryFunctions.UpdateAsync(command,connectionString);
            }
        }


        public static async Task<bool> UpdateProductQuantity(int productID, int quentity)
        {
            using (SqlCommand command = new SqlCommand("Sp_UpdateProductQuantity"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductID", productID);
               
                command.Parameters.AddWithValue("@Quantity", quentity);
               

                return await clsPrimaryFunctions.UpdateProductQuantityAsync(command, connectionString);
            }
        }
    }

}