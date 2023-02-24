using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataProvider
{
    public class AccessConnectionDB
    {
        private OleDbConnectionStringBuilder connectionString;
        private OleDbDataAdapter accessData;
        public string AccessConnectionsString { get => connectionString.ConnectionString.ToString(); }
        
        
        private OleDbConnection dbConnection;

        public event Action<string> ConnectionState;

        // Provider=Microsoft.ACE.OLEDB.12.0;
        // Data Source=F:\SkillboxUnityCourse\Module3\Practice16\AdoNet\AdoNet\bin\Debug\net7.0-windows\AccessDB.mdb
        // Provider=Microsoft.ACE.OLEDB.12.0;
        // Data Source = F:\SkillboxUnityCourse\Module3\Practice16\AdoNet\AdoNet\bin\Debug\net7.0-windows\AccessDB.accdb;
        // Persist Security Info=True

        public AccessConnectionDB()
        {
            connectionString = new OleDbConnectionStringBuilder()
            {
                Provider = "Microsoft.ACE.OLEDB.12.0",
                DataSource = @"AccessDB.accdb",
                PersistSecurityInfo = true
            };

            InitializingDBAdapter();
        }

        private void InitializingDBAdapter()
        {
            accessData = new OleDbDataAdapter();

            //accessData.SelectCommand = new OleDbCommand("SELECT * FROM Purchases", dbConnection);
        }

        #region проверка соединения
        public void OpenConnection()
        {
            using (dbConnection = new OleDbConnection(connectionString.ToString()))
            {
                try
                {
                    dbConnection.Open();
                    ConnectionState?.Invoke(dbConnection.State.ToString());
                    Thread.Sleep(2000);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            ConnectionState?.Invoke(dbConnection.State.ToString());
        }

        public async void OpenConnectionAsync()
        {
            await Task.Factory.StartNew(OpenConnection);
        }

        #endregion

        #region Получить все записи покупок
        public DataTable GetAllPurchases()
        {
            DataTable dt = new DataTable();
            using (dbConnection = new OleDbConnection(connectionString.ConnectionString))
            {
                accessData.SelectCommand = new OleDbCommand(@"SELECT * FROM Purchases", dbConnection);

                accessData.Fill(dt);
            }
            return dt;
        }

        #endregion
    }
}
