using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AdoNet.Infrastructure;
using AdoNet.Models;
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

        #region Выбранный клиент
        private DataRowView selectedClient;
        public DataRowView SelectedClient
        {
            get => selectedClient;
            set
            {
                Set(ref selectedClient, value);
                if (value != null)
                {
                    GetSelectedClientPurchases(value.Row.ItemArray[5].ToString());
                }
                if (value == null)
                {
                    Purchases = null;
                }
                
            }
        }
        #endregion

        #region Новый клиент
        private Client newClient = new Client();
        public Client NewClient
        {
            get => newClient;
            set => Set(ref newClient, value);
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

        #region Свойства для новой покупки
        private string itemCode;
        public string ItemCode
        {
            get => itemCode;
            set => Set(ref itemCode, value);
        }

        private string itemName;
        public string ItemName
        {
            get => itemName;
            set => Set(ref itemName, value);
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

            GetAllClientsCommand = new Command(OnGetAllClientsCommandExecute,
                                               CanGetAllClientsCommandExecute);

            AddNewClientCommand = new Command(OnAddNewClientCommandExecute,
                                              CanAddNewClientCommandExecute);

            CellEditEndCommand = new Command(OnCellEditEndCommandExcute, null);

            ClientCellChangedCommand = new Command(OnClientCellChangedCommandExecute, null);

            DeleteClientRecordCommand = new Command(OnDeleteClientRecordExecute,
                                                    CanDeleteClientRecordCommandExecute);

            AddNewPurchase = new Command(OnAddNewPurchaseExecute,
                                         CanAddNewPurchaseExecute);

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
        private void GetSelectedClientPurchases(string clientEmail)
        {
            Purchases = accessConnection.GetClientPurchases(clientEmail); 
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
        }
        private bool CanGetAllClientsCommandExecute(object p)
            => SqlConnectionStatus == ConnectionState.Closed.ToString() || SqlConnectionStatus == null;

        #endregion

        #region Добавить клиента
        public ICommand AddNewClientCommand { get; }

        private void OnAddNewClientCommandExecute(object p)
        {
            DataTable tmpTable;

            if (Clients == null)
            {
                tmpTable = sqlConnection.GetClients();
            }
            else
            {
                tmpTable = Clients;
            }

            // Плохое решение, при изменении структуры таблица придеться дописывать
            DataRow client = tmpTable.NewRow();
            client[1] = NewClient.Name;
            client[2] = NewClient.Surname;
            client[3] = NewClient.Patronymic;
            client[4] = NewClient.Phone;
            client[5] = NewClient.Email;

            tmpTable.Rows.Add(client);

            sqlConnection.UpdateDBInformationAsync(tmpTable);
        }

        private bool CanAddNewClientCommandExecute(object p)
        {
            return true;
        }

        #endregion

        #region Удалить клиента
        public ICommand DeleteClientRecordCommand { get; }

        private void OnDeleteClientRecordExecute(object p)
        {
            ((DataRowView)p).Row.Delete();
            sqlConnection.UpdateDBInformationAsync(Clients);
        }

        private bool CanDeleteClientRecordCommandExecute(object p) => p != null;
        #endregion

        #region Доавить покупку

        public ICommand AddNewPurchase { get; }
        private void OnAddNewPurchaseExecute(object p)
        {
            DataRow purchase = Purchases.NewRow();
            purchase[1] = SelectedClient["eMail"];
            purchase[2] = ItemCode;
            purchase[3] = ItemName;

            Purchases.Rows.Add(purchase);
            accessConnection.AddNewPurchase(Purchases);
        }
        private bool CanAddNewPurchaseExecute(object p) => p != null;

        #endregion

        #endregion

        #region Prism Commands as Event

        #region Изменение клиента по завершении редактирования ячейки
        public ICommand CellEditEndCommand { get; }

        private void OnCellEditEndCommandExcute(object p)
        {
            SelectedClient.Row.BeginEdit();
            sqlConnection.UpdateDBInformationAsync(Clients);
        }

        #endregion

        #region Изменение клиента при смене ячейки
        public ICommand ClientCellChangedCommand { get; }

        private void OnClientCellChangedCommandExecute(object p)
        {
            if (SelectedClient == null)
            {
                return;
            }
            SelectedClient.Row.EndEdit();
            sqlConnection.UpdateDBInformationAsync(Clients);
        }

        #endregion

        #endregion
    }

}
