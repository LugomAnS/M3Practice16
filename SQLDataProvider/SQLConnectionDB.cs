using System.Data.SqlClient;
using System.Diagnostics;

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
            connection = new SqlConnection(connString.ConnectionString);
            connection.StateChange += Connection_StateChange;
        }

        public void OpenConnection()
        {
            try
            {
                connection.Open();
                Thread.Sleep(2000);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public async void OpenConnectionAsync()
        {
            await Task.Factory.StartNew(OpenConnection);
        }

        // сообщаем подписчикам об изменении статуса соединения
        private void Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            ConnectionState?.Invoke((sender as SqlConnection).State.ToString());
        }
    }
}