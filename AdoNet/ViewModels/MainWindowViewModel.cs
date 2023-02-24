using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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


        #region Данные о клиентах
        private DataTable clients;
        public DataTable Clients
        {
            get => clients;
            set => Set(ref clients, value);
        }
        #endregion

        #region Данные о покупках
        private DataTable purchases;
        public DataTable Purchases
        {
            get => purchases;
            set => Set(ref purchases, value);
        }
        #endregion

        #region Статус обработки запроса
        private string requestStatus;
        public string RequestStatus
        {
            get => requestStatus;
            set => Set(ref requestStatus, value);
        }
        #endregion
        #endregion

        public MainWindowViewModel()
        {
            SQLConnectionSet = new Command(OnSQLConnectionSetExecute,
                                           CanSQLConnectionSetExecute);

            GetAllClientsCommand = new Command(OnGetAllClientsCommandExecute, null);

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

        #region Получить клиентов
        public ICommand GetAllClientsCommand { get; }
        private void OnGetAllClientsCommandExecute(object p)
        {
            Clients = sqlConnection.GetClients();
            Purchases = accessConnection.GetAllPurchases();
        }
        #endregion

        #endregion
    }

}
