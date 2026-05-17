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
    public static class clsTransactionsData
    {
        static string connectionString = clsDataAccessSettings.ConnectionString;

        public static async Task<int?> AddNewTransactions(int ProductID, int UserID, enTransactionType Type, int Quantity, DateTime TransactionDate)
        {
            using (SqlCommand command = new SqlCommand("Sp_AddNewTransactions"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductID", ProductID);
                command.Parameters.AddWithValue("@UserID", UserID);
                command.Parameters.AddWithValue("@Type", Type);
                command.Parameters.AddWithValue("@Quantity", Quantity);
                command.Parameters.AddWithValue("@TransactionDate", TransactionDate);
                command.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;
                string ID = "@ID";

                return await clsPrimaryFunctions.AddAsync(command, connectionString, ID);
            }
        }

        public static async Task<bool> DeleteTransactions(int TransactionID)
        {
            using (SqlCommand command = new SqlCommand("Sp_DeleteTransactions"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", TransactionID);
                return await clsPrimaryFunctions.DeleteAsync(command, connectionString);
            }
        }

        public static async Task<List<TransactionModel?>> GetAllTransactions()
        {
            using (SqlCommand command = new SqlCommand("Sp_GetAllTransactions"))
            {
                List<TransactionModel> transactions = new List<TransactionModel>();
                command.CommandType = CommandType.StoredProcedure;
                using (var reader = await clsPrimaryFunctions.GetAsync(command, connectionString)) {

                    while (await reader.ReadAsync())
                    {
                        transactions.Add(new TransactionModel
                        {
                            TransactionID = reader.GetInt32(0),
                            ProductID = reader.GetInt32(1),
                            UserID = reader.GetInt32(2),
                            Type =(enTransactionType) reader.GetInt32(3),
                            Quantity = reader.GetInt32(4),
                            TransactionDate = reader.GetDateTime(5)
                        });
                    }
                }
                return transactions;
            }
            
        }

        public async static Task<TransactionModel?> FindByID(int TransactionID)
        {
            TransactionModel? transaction = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("Sp_GetTransactionsByID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID", TransactionID);

                       
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                transaction = new TransactionModel
                                {
                                    TransactionID = TransactionID, 
                                    ProductID = (int)reader["ProductID"],
                                    UserID = (int)reader["UserID"],
                                    Type = (enTransactionType)reader["Type"],
                                    Quantity = (int)reader["Quantity"],
                                    TransactionDate = (DateTime)reader["TransactionDate"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsPrimaryFunctions.EntireInfoToEventLoge(ex.Message);
                
                return null;
            }

            return transaction;
        }

        public static async Task<bool> UpdateTransactions(int TransactionID, int ProductID, int UserID, enTransactionType Type, int Quantity, DateTime TransactionDate)
        {
            using (SqlCommand command = new SqlCommand("Sp_UpdateTransactions"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TransactionID", TransactionID);
                command.Parameters.AddWithValue("@ProductID", ProductID);
                command.Parameters.AddWithValue("@UserID", UserID);
                command.Parameters.AddWithValue("@Type", Type);
                command.Parameters.AddWithValue("@Quantity", Quantity);
                command.Parameters.AddWithValue("@TransactionDate", TransactionDate);
                // داخل دالة _AddNewTransactions أو _UpdateTransactions
               

                return await clsPrimaryFunctions.UpdateAsync(command,connectionString);
            }
        }

    }
}
