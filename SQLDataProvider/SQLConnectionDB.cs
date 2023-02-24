using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace DataProvider
{
    public class SQLConnectionDB
    {
        private SqlConnectionStringBuilder connString;
        private SqlDataAdapter sqlData;
        public string SQLConnectionString { get => connString.ConnectionString.ToString(); }
        
        
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
            InititialSQLAdapter();
        }

        private void InititialSQLAdapter()
        {
            sqlData = new SqlDataAdapter();

            sqlData.SelectCommand = new SqlCommand(@"SELECT * FROM Clients", connection);
        }

        // сообщаем подписчикам об изменении статуса соединения
        private void Connection_StateChange(object sender, StateChangeEventArgs e)
        {
            ConnectionState?.Invoke((sender as SqlConnection).State.ToString());
        }

        // проверка связи
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

        public DataTable GetClients()
        {
            DataTable dt = new DataTable();

            try
            {
                connection.Open();
                sqlData.Fill(dt);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                connection.Close();
            }

            return dt;
        }
        // TODO - не работает
        // WaitingForCalling - разобраться что ждет
        public async Task<DataTable> GetClientsAsync()
        {
            return await Task<DataTable>.Factory.StartNew(this.GetClients);
        }

    }
}