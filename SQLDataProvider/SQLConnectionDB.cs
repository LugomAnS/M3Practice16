using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataProvider
{
    public class SQLConnectionDB
    {
        private SqlConnectionStringBuilder connString;
        private SqlConnection connection;

        /// <summary>
        /// Сообщает об изменении статуса соединения с БД
        /// </summary>
        public event Action<string> ConnectionState;

        public SQLConnectionDB()
        {
            connString = new SqlConnectionStringBuilder()
            {
                DataSource = @"(localdb)\mssqllocaldb",
                InitialCatalog = "ADONETTestDB",
                IntegratedSecurity = true
            };

            connection.StateChange += Connection_StateChange;
        }

        public string OpenConnection()
        {
            connection = new SqlConnection() { ConnectionString = connString.ConnectionString };

            connection.StateChange += Connection_StateChange;

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

        private void Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            ConnectionState?.Invoke((sender as SqlConnection).State.ToString());
        }
    }
}