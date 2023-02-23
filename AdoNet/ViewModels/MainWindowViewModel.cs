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

        #region SQL строка соединения
        public string SQLConnectionString { get; set; }
        #endregion

        #region Статус Access соединения
        private string accessConnectionStatus;
        public string AccessConnectionStatus
        {
            get => accessConnectionStatus;
            set => Set(ref accessConnectionStatus, value);
        }
        #endregion

        #region Access соединение
        private AccessConnectionDB accessConnection;
        #endregion

        #region Access строка соединение
        public string AccessConnectionString { get; set; }
        #endregion

        #endregion

        public MainWindowViewModel()
        {
            SQLConnectionSet = new Command(OnSQLConnectionSetExecute,
                                           CanSQLConnectionSetExecute);

            sqlConnection = new SQLConnectionDB();
            SQLConnectionString = sqlConnection.SQLConnectionString;
            sqlConnection.ConnectionState += SQLConnectionStatusChange;

            accessConnection = new AccessConnectionDB();
            AccessConnectionString = accessConnection.AccessConnectionsString;
            accessConnection.ConnectionState += AccessConnectionStatusChange;
        }

        private void SQLConnectionStatusChange(string status)
        {
            SqlConnectionStatus = status;
        }
        private void AccessConnectionStatusChange(string status)
        {
            AccessConnectionStatus = status;
        }

        #region Команды

        #region Установить соединение
        public ICommand SQLConnectionSet { get; }
        private void OnSQLConnectionSetExecute(object p)
        {
            sqlConnection.OpenConnectionAsync();
            accessConnection.OpenConnectionAsync();
        }
        private bool CanSQLConnectionSetExecute(object p) => true;

        #endregion

        #endregion
    }
}
