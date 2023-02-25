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
            InitSQLAdapter();
        }

        private void InitSQLAdapter()
        {
            sqlData = new SqlDataAdapter();

            #region SELECT
            string sql = @"SELECT * FROM Clients";
            sqlData.SelectCommand = new SqlCommand(sql, connection);
            #endregion

            #region UPDATE
            sql = @"UPDATE Clients SET
                           clientName = @clientName,
                           clientSurname = @clientSurname,
                           clientPatronymic = @clientPatronymic,
                           phone = @phone,
                           eMail = @eMail
                        WHERE id = @id";
            sqlData.UpdateCommand = new SqlCommand(sql, connection);
            sqlData.UpdateCommand.Parameters.Add("@id", SqlDbType.Int, 0, "id").SourceVersion = DataRowVersion.Original;
            sqlData.UpdateCommand.Parameters.Add("@clientName", SqlDbType.NVarChar, 15, "clientName");
            sqlData.UpdateCommand.Parameters.Add("@clientSurname", SqlDbType.NVarChar, 15, "clientSurname");
            sqlData.UpdateCommand.Parameters.Add("@clientPatronymic", SqlDbType.NVarChar, 15, "clientPatronymic");
            sqlData.UpdateCommand.Parameters.Add("@phone", SqlDbType.NVarChar, 15, "phone");
            sqlData.UpdateCommand.Parameters.Add("@eMail", SqlDbType.NVarChar, 20, "eMail");
            #endregion

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


        #region Получить всех клиентов
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
        #endregion

        #region Изменение клиента

        public void UpdateClientInfo(object clientsData)
        {
            try
            {
                connection.Open();
                sqlData.Update(clientsData as DataTable);
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

        public async void UpdateClientInfoAsync(object clientData)
        {
            await Task.Factory.StartNew(UpdateClientInfo, clientData);
        }

        #endregion

    }
}