using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider
{
    public class AccessConnectionDB
    {
        private OleDbConnectionStringBuilder connectionString;
        public string AccessConnectionsString { get => connectionString.ConnectionString.ToString(); }
        
        
        private OleDbConnection dbConnection;

        public event Action<string> ConnectionState;

        // Provider=Microsoft.ACE.OLEDB.12.0;
        // Data Source=F:\SkillboxUnityCourse\Module3\Practice16\AdoNet\AdoNet\bin\Debug\net7.0-windows\AccessDB.mdb

        public AccessConnectionDB()
        {
            connectionString = new OleDbConnectionStringBuilder()
            {
                Provider = "Microsoft.ACE.OLEDB.12.0",
                DataSource = @"AccessDB.mdb"
            };
        }

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
    }
}
