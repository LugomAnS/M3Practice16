using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AdoNet.Infrastructure;
using DataProvider;

namespace AdoNet.ViewModels
{
    internal class MainWindowViewModel : INPC
    {
        #region Поля и свойства

        #region Статус SQL соединения
        private string sqlConnectionStatus;
        public string SqlConnectionStatus
        {
            get => sqlConnectionStatus;
            set => Set(ref sqlConnectionStatus, value);
        }
        #endregion

        #region SQL соединение
        private SQLConnectionDB sqlConnection;
        #endregion

        #endregion

        public MainWindowViewModel()
        {
            SQLConnectionSet = new Command(OnSQLConnectionSetExecute,
                                           CanSQLConnectionSetExecute);

            sqlConnection = new SQLConnectionDB();
            sqlConnection.ConnectionState += SQLConnectionStatusChange;
        }

        private void SQLConnectionStatusChange(string status)
        {
            SqlConnectionStatus = status;
        }

        #region Команды

        #region Установить соединение
        public ICommand SQLConnectionSet { get; }
        private void OnSQLConnectionSetExecute(object p)
        {
            sqlConnection.OpenConnection();         
        }
        private bool CanSQLConnectionSetExecute(object p) => true;

        #endregion

        #endregion
    }
}
