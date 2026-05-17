using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace Inventory.DataAccess
{
    public class clsPrimaryFunctions
    {
        public static async Task<int?> AddAsync(SqlCommand command, string ConnectionString, string outputParameterName)
        {
            int? ID = null;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                command.Connection = connection;

                try
                {
                    await connection.OpenAsync();

                    // تنفيذ الإجراء المخزن. بما أننا نتوقع معامل إخراج، نستخدم ExecuteNonQueryAsync().
                    await command.ExecuteNonQueryAsync();

                    // قراءة قيمة معامل الإخراج المحدد بواسطة 'outputParameterName'
                    SqlParameter? outputParam = command.Parameters[outputParameterName] as SqlParameter;

                    if (outputParam != null && outputParam.Value != DBNull.Value)
                    {
                        // حاول التحويل إلى int
                        if (int.TryParse(outputParam.Value.ToString(), out int insertedID))
                        {
                            ID = insertedID;
                        }
                        else
                        {
                            throw new Exception($"Output parameter '{outputParameterName}' is not a valid integer value.");
                        }
                    }
                    else
                    {
                        throw new Exception($"Output parameter '{outputParameterName}' is null or not found in the command parameters.");
                    }
                }
                catch (Exception ex)
                {
                    throw; 
                }
                return ID;
            }
        }

        public static async Task<bool> DeleteAsync(SqlCommand command, string ConnectionString)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                command.Connection = connection;

                try
                {
                    await connection.OpenAsync();
                    int rowAffected = await command.ExecuteNonQueryAsync();

                    // If your SP did NOT use RAISERROR for @@ROWCOUNT=0,
                    // this line would determine success/failure.
                    // However, with RAISERROR, rowAffected will either be >0 or an exception is thrown.
                    return rowAffected > 0;
                }
                catch (SqlException sqlEx)
                {

                    // Check if this specific SqlException is the one from your RAISERROR for no records found.
                    // You might check sqlEx.Message, or if you use specific error numbers in RAISERROR.
                    // For example, if you set RAISERROR('...', 16, 1, 50001); you could check sqlEx.Number == 50001
                    if (sqlEx.Message.Contains("No records found to delete for the provided ID."))
                    {
                        // If the SP explicitly said no records were found, return false.
                        // Or, if your business logic demands, you could throw KeyNotFoundException here.
                        // For a Delete operation, returning false is often a clean way to indicate "not deleted because not found".
                        return false;
                    }
                    else
                    {
                        // For any other SQL-related error, re-throw the specific SqlException
                        // (or a custom exception wrapping it)
                        throw; // Re-throws the original SqlException
                    }
                }
                catch (Exception ex)
                {

                    throw; // Re-throws the original exception
                }
            }
        }


        public static async Task<bool> UpdateAsync(SqlCommand command, string ConnectionString) // ترجع bool لتدل على عدد الصفوف المتأثرة
        {
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                command.Connection = connection;

                try
                {
                    await connection.OpenAsync();
                    rowAffected = await command.ExecuteNonQueryAsync();
                }
                catch (SqlException sqlEx) // التقاط أخطاء SQL المحددة
                {

                    throw;
                }
                catch (Exception ex) // التقاط أي استثناءات أخرى غير SqlException
                {

                    // رمي استثناء جديد أو إعادة رمي الأصلي كخطأ عام غير متوقع
                    throw new Exception("An unexpected error occurred during the update database operation.", ex);
                }
            }

            return rowAffected > 0;
        }


        public static async Task<SqlDataReader> GetAsync(SqlCommand command, string connectionString)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            command.Connection = connection;
            // CommandBehavior.CloseConnection يضمن إغلاق الاتصال عند إغلاق الـ DataReader

            SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            return reader;
        }


        //update product quantity after transaction

        public static async Task<bool> UpdateProductQuantityAsync(SqlCommand command, string connectionString)
        {
            int rowAffected = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                command.Connection = connection;

                try
                {
                    await connection.OpenAsync();
                    rowAffected = await command.ExecuteNonQueryAsync();
                }
                catch (SqlException sqlEx) // التقاط أخطاء SQL المحددة
                {

                    throw;
                }
                catch (Exception ex) // التقاط أي استثناءات أخرى غير SqlException
                {

                    // رمي استثناء جديد أو إعادة رمي الأصلي كخطأ عام غير متوقع
                    throw new Exception("An unexpected error occurred during the update product quentity database operation.", ex);
                }
                return rowAffected > 0;
            }
        }



        public static void EntireInfoToEventLoge(string message)
    {
        string source = "SmartInventoryApp"; // اسم نظامك
        string log = "Application";

        try
        {
            // التأكد من وجود المصدر في النظام
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, log);
            }

            // كتابة الخطأ في الـ Event Viewer
            EventLog.WriteEntry(source, message, EventLogEntryType.Error);
        }
        catch
        {

            Console.WriteLine("Logging failed: " + message);
        }
    }
}
      
}