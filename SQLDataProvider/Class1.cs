using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataProvider
{
    public class SQLConnectionDB
    {
        private SqlConnectionStringBuilder connString;

        public SQLConnectionDB()
        {
            connString = new SqlConnectionStringBuilder()
            {
                DataSource = @"(localdb)\mssqllocaldb",
                InitialCatalog = "ADONETTestDB",
                IntegratedSecurity = true
            };
        }

        public string OpenConnection()
        {
            SqlConnection connection = new SqlConnection() { ConnectionString = connString.ConnectionString };

            try
            {
                connection.Open();
                return "Соединение установлено";
            }
            catch (Exception)
            {
                return "Исключение";
            }
            finally
            {
                connection.Close();
            }
        }
    }
}