
using System.Data;
using Microsoft.Data.SqlClient;
using Inventory.DTOs.Model;


namespace Inventory.DataAccess
{
        public static class clsCategoriesData
        {
         static string connectionString = clsDataAccessSettings.ConnectionString;
        public static async Task<int?> AddNewCategories(string CategoryName)
            {
                using (SqlCommand command = new SqlCommand("Sp_AddNewCategories"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CategoryName", CategoryName);
                command.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;
                     string ID = "@ID";


                return await clsPrimaryFunctions.AddAsync(command, connectionString, ID);
                }
            }

            public static async Task<bool> DeleteCategories(int CategoryID)
            {
                using (SqlCommand command = new SqlCommand("Sp_DeleteCategories"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", CategoryID);
                    return await clsPrimaryFunctions.DeleteAsync(command, connectionString);
                }
            }

        public static async Task<List<CategoryModel>> GetAllCategories()
        {
            using (SqlCommand command = new SqlCommand("Sp_GetAllCategories"))
            {
                command.CommandType = CommandType.StoredProcedure;
                var categories = new List<CategoryModel>();
                using (var reader = await clsPrimaryFunctions.GetAsync(command, connectionString))
                {
                    while (await reader.ReadAsync())
                    {
                        categories.Add(new CategoryModel
                        {
                            CategoryID = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 0,
                            CategoryName = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : string.Empty
                        });
                    }
                }
                return categories;
            }
        }

        public static async Task<CategoryModel?> FindByID(int CategoryID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("Sp_GetCategoriesByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", CategoryID);

                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // ننشئ الكائن فقط إذا وجدنا سجل في قاعدة البيانات
                                return new CategoryModel
                                {
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    CategoryName = reader["CategoryName"] as string ?? string.Empty
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        clsPrimaryFunctions.EntireInfoToEventLoge(ex.Message);
                        return null;
                    }
                }
            }
            return null; // إذا لم يجد شيئاً
        }
        public static async Task<bool> UpdateCategories(int CategoryID, string CategoryName)
            {
                using (SqlCommand command = new SqlCommand("Sp_UpdateCategories"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CategoryID", CategoryID);
                    command.Parameters.AddWithValue("@CategoryName", CategoryName);

                    return await clsPrimaryFunctions.UpdateAsync(command, connectionString);
                }
            }

        }

}
