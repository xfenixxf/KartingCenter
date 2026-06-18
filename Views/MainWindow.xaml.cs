using KartingCenter.Models;
using KartingCenter.Services;
using KartingCenter.Windows;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KartingCenter
{
    public partial class MainWindow : Window
    {
        private ApiService _api;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _api = new ApiService("http://localhost:55493/");
            await LoadAllData();
            await LoadDashboardStats();
            StatusText.Text = "Статус: Подключено ✅";
            StatusBarText.Text = "Соединение с API установлено";

            SetTabsVisibilityByRole();
        }

        #region Управление видимостью вкладок и кнопок по ролям

        private void SetTabsVisibilityByRole()
        {
            var user = App.Current.Properties["CurrentUser"] as UserInfoDTO;
            if (user == null)
            {
                HideAllTabs();
                TabClients.Visibility = Visibility.Visible;
                ExportImportPanel.Visibility = Visibility.Collapsed;
                return;
            }

            string role = user.Role?.ToLower() ?? "";
            HideAllTabs();

            if (role == "владелец" || role == "owner")
            {
                ShowAllTabs();
                ExportImportPanel.Visibility = Visibility.Visible; 
            }
            else if (role == "администратор" || role == "admin")
            {
                TabClients.Visibility = Visibility.Visible;
                TabTeams.Visibility = Visibility.Visible;
                TabLocations.Visibility = Visibility.Visible;
                TabRides.Visibility = Visibility.Visible;
                TabParticipants.Visibility = Visibility.Visible;
                TabPersonal.Visibility = Visibility.Visible;
                ExportImportPanel.Visibility = Visibility.Collapsed;
            }
            else if (role == "механик" || role == "mechanic")
            {
                TabCartTypes.Visibility = Visibility.Visible;
                TabKarts.Visibility = Visibility.Visible;
                TabParts.Visibility = Visibility.Visible;
                TabTO.Visibility = Visibility.Visible;
                TabTypeTO.Visibility = Visibility.Visible;
                ExportImportPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                TabClients.Visibility = Visibility.Visible;
                ExportImportPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void HideAllTabs()
        {
            TabClients.Visibility = Visibility.Collapsed;
            TabTeams.Visibility = Visibility.Collapsed;
            TabCartTypes.Visibility = Visibility.Collapsed;
            TabKarts.Visibility = Visibility.Collapsed;
            TabLocations.Visibility = Visibility.Collapsed;
            TabRides.Visibility = Visibility.Collapsed;
            TabParticipants.Visibility = Visibility.Collapsed;
            TabParts.Visibility = Visibility.Collapsed;
            TabTO.Visibility = Visibility.Collapsed;
            TabTypeTO.Visibility = Visibility.Collapsed;
            TabPersonal.Visibility = Visibility.Collapsed;
        }

        private void ShowAllTabs()
        {
            TabClients.Visibility = Visibility.Visible;
            TabTeams.Visibility = Visibility.Visible;
            TabCartTypes.Visibility = Visibility.Visible;
            TabKarts.Visibility = Visibility.Visible;
            TabLocations.Visibility = Visibility.Visible;
            TabRides.Visibility = Visibility.Visible;
            TabParticipants.Visibility = Visibility.Visible;
            TabParts.Visibility = Visibility.Visible;
            TabTO.Visibility = Visibility.Visible;
            TabTypeTO.Visibility = Visibility.Visible;
            TabPersonal.Visibility = Visibility.Visible;
        }

        #endregion

        #region Загрузка данных

        private async Task LoadAllData()
        {
            await LoadClients();
            await LoadTeams();
            await LoadCartTypes();
            await LoadKarts();
            await LoadLocations();
            await LoadRides();
            await LoadParticipants();
            await LoadParts();
            await LoadTO();
            await LoadTypeTO();
            await LoadPersonal();
        }

        private async Task LoadClients()
        {
            var clients = await _api.GetClientsAsync();
            ClientsGrid.ItemsSource = clients;
        }

        private async Task LoadTeams()
        {
            var teams = await _api.GetTeamsAsync();
            TeamsGrid.ItemsSource = teams;
        }

        private async Task LoadCartTypes()
        {
            var cartTypes = await _api.GetCartTypesAsync();
            CartTypesGrid.ItemsSource = cartTypes;
        }

        private async Task LoadKarts()
        {
            var karts = await _api.GetKartsAsync();
            KartsGrid.ItemsSource = karts;
        }

        private async Task LoadLocations()
        {
            var locations = await _api.GetLocationsAsync();
            LocationsGrid.ItemsSource = locations;
        }

        private async Task LoadRides()
        {
            var rides = await _api.GetRidesAsync();
            RidesGrid.ItemsSource = rides;
        }

        private async Task LoadParticipants()
        {
            var participants = await _api.GetRideParticipantsAsync();
            ParticipantsGrid.ItemsSource = participants;
        }

        private async Task LoadParts()
        {
            var parts = await _api.GetPartsAsync();
            PartsGrid.ItemsSource = parts;
        }

        private async Task LoadTO()
        {
            var tos = await _api.GetTOAsync();
            TOGrid.ItemsSource = tos;
        }

        private async Task LoadTypeTO()
        {
            var types = await _api.GetTypeTOAsync();
            TypeTOGrid.ItemsSource = types;
        }

        private async Task LoadPersonal()
        {
            var personal = await _api.GetPersonalAsync();
            PersonalGrid.ItemsSource = personal;
        }

        #endregion

        #region Dashboard

        private async Task LoadDashboardStats()
        {
            var rides = await _api.GetRidesAsync();
            int todayRidesCount = rides.Count(r => r.RideDate.Date == DateTime.Today.Date);

            string stickerColor, statusDescription;
            if (todayRidesCount < 5)
            {
                stickerColor = "Зелёный";
                statusDescription = "Низкая загрузка, мало клиентов";
            }
            else if (todayRidesCount <= 15)
            {
                stickerColor = "Жёлтый";
                statusDescription = "Нормальная загрузка";
            }
            else
            {
                stickerColor = "Красный";
                statusDescription = "Пиковая загрузка, высокая нагрузка на технику и персонал";
            }

            StatsText.Text = $"Заездов сегодня: {todayRidesCount} | {statusDescription}";

            Brush color = Brushes.Gray;
            switch (stickerColor)
            {
                case "Зелёный": color = Brushes.Green; break;
                case "Жёлтый": color = Brushes.Gold; break;
                case "Красный": color = Brushes.Red; break;
            }
            StickerIndicator.Background = color;
        }

        #endregion

        #region Обработчики клиентов

        private async void AddClientBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new ClientWindow(_api);
            if (window.ShowDialog() == true) { await LoadClients(); StatusBarText.Text = "Список клиентов обновлён"; }
        }

        private async void EditClientBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Client selected)
            {
                var window = new ClientWindow(_api, selected);
                if (window.ShowDialog() == true) { await LoadClients(); StatusBarText.Text = "Клиент обновлён"; }
            }
            else MessageBox.Show("Выберите клиента для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void DeleteClientBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Client selected)
            {
                if (MessageBox.Show($"Удалить {selected.FullName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await _api.DeleteClientAsync(selected.Id);
                    await LoadClients();
                    StatusBarText.Text = "Клиент удалён";
                }
            }
            else MessageBox.Show("Выберите клиента для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RefreshClientsBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadClients();
            StatusBarText.Text = "Список клиентов обновлён";
        }

        #endregion

        #region Обработчики команд

        private async void AddTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new TeamWindow(_api);
            if (window.ShowDialog() == true) { await LoadTeams(); StatusBarText.Text = "Список команд обновлён"; }
        }

        private async void EditTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TeamsGrid.SelectedItem is Team selected)
            {
                var window = new TeamWindow(_api, selected);
                if (window.ShowDialog() == true) { await LoadTeams(); StatusBarText.Text = "Команда обновлена"; }
            }
            else MessageBox.Show("Выберите команду для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void DeleteTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TeamsGrid.SelectedItem is Team selected)
            {
                if (MessageBox.Show($"Удалить команду {selected.Name}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await _api.DeleteTeamAsync(selected.Id);
                    await LoadTeams();
                    StatusBarText.Text = "Команда удалена";
                }
            }
            else MessageBox.Show("Выберите команду для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RefreshTeamsBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadTeams();
            StatusBarText.Text = "Список команд обновлён";
        }

        #endregion

        #region Обработчики типов картов

        private async void AddCartTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new CartTypeWindow(_api);
            if (window.ShowDialog() == true) { await LoadCartTypes(); StatusBarText.Text = "Список типов картов обновлён"; }
        }

        private async void EditCartTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CartTypesGrid.SelectedItem is CartType selected)
            {
                var window = new CartTypeWindow(_api, selected);
                if (window.ShowDialog() == true) { await LoadCartTypes(); StatusBarText.Text = "Тип карта обновлён"; }
            }
            else MessageBox.Show("Выберите тип карта для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void DeleteCartTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CartTypesGrid.SelectedItem is CartType selected)
            {
                if (MessageBox.Show($"Удалить тип карта {selected.Name}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await _api.DeleteCartTypeAsync(selected.Id);
                    await LoadCartTypes();
                    StatusBarText.Text = "Тип карта удалён";
                }
            }
            else MessageBox.Show("Выберите тип карта для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RefreshCartTypesBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadCartTypes();
            StatusBarText.Text = "Список типов картов обновлён";
        }

        #endregion

        #region Обработчики картов

        private async void AddKartBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new KartWindow(_api);
            if (window.ShowDialog() == true) { await LoadKarts(); StatusBarText.Text = "Список картов обновлён"; }
        }

        private async void EditKartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (KartsGrid.SelectedItem is Kart selected)
            {
                var window = new KartWindow(_api, selected);
                if (window.ShowDialog() == true) { await LoadKarts(); StatusBarText.Text = "Карт обновлён"; }
            }
            else MessageBox.Show("Выберите карт для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void DeleteKartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (KartsGrid.SelectedItem is Kart selected)
            {
                if (MessageBox.Show($"Удалить карт {selected.SerialNumber}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await _api.DeleteKartAsync(selected.Id);
                    await LoadKarts();
                    StatusBarText.Text = "Карт удалён";
                }
            }
            else MessageBox.Show("Выберите карт для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RefreshKartsBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadKarts();
            StatusBarText.Text = "Список картов обновлён";
        }

        #endregion

        #region Обработчики локаций

        private async void AddLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new LocationWindow(_api);
            if (window.ShowDialog() == true) { await LoadLocations(); StatusBarText.Text = "Список локаций обновлён"; }
        }

        private async void EditLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LocationsGrid.SelectedItem is Location selected)
            {
                var window = new LocationWindow(_api, selected);
                if (window.ShowDialog() == true) { await LoadLocations(); StatusBarText.Text = "Локация обновлена"; }
            }
            else MessageBox.Show("Выберите локацию для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void DeleteLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LocationsGrid.SelectedItem is Location selected)
            {
                if (MessageBox.Show($"Удалить локацию {selected.Name}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await _api.DeleteLocationAsync(selected.Id);
                    await LoadLocations();
                    StatusBarText.Text = "Локация удалена";
                }
            }
            else MessageBox.Show("Выберите локацию для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RefreshLocationsBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadLocations();
            StatusBarText.Text = "Список локаций обновлён";
        }

        #endregion

        #region Обработчики заездов

        private async void AddRideBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new RideWindow(_api);
            if (window.ShowDialog() == true) { await LoadRides(); await LoadDashboardStats(); StatusBarText.Text = "Список заездов обновлён"; }
        }

        private async void EditRideBtn_Click(object sender, RoutedEventArgs e)
        {
            if (RidesGrid.SelectedItem is Ride selected)
            {
                var window = new RideWindow(_api, selected);
                if (window.ShowDialog() == true) { await LoadRides(); await LoadDashboardStats(); StatusBarText.Text = "Заезд обновлён"; }
            }
            else MessageBox.Show("Выберите заезд для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void DeleteRideBtn_Click(object sender, RoutedEventArgs e)
        {
            if (RidesGrid.SelectedItem is Ride selected)
            {
                if (MessageBox.Show($"Удалить заезд #{selected.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await _api.DeleteRideAsync(selected.Id);
                    await LoadRides();
                    await LoadDashboardStats();
                    StatusBarText.Text = "Заезд удалён";
                }
            }
            else MessageBox.Show("Выберите заезд для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RefreshRidesBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadRides();
            StatusBarText.Text = "Список заездов обновлён";
        }

        private async void FilterDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterDatePicker.SelectedDate.HasValue)
            {
                var rides = await _api.GetRidesByDateAsync(FilterDatePicker.SelectedDate.Value);
                RidesGrid.ItemsSource = rides;
                StatusBarText.Text = $"Отфильтровано по дате: {FilterDatePicker.SelectedDate.Value:dd.MM.yyyy}";
            }
        }

        private async void ShowAllRidesBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadRides();
            FilterDatePicker.SelectedDate = null;
            StatusBarText.Text = "Показаны все заезды";
        }

        private async void MonthFilterPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MonthFilterPicker.SelectedDate.HasValue)
            {
                var date = MonthFilterPicker.SelectedDate.Value;
                var rides = await _api.GetRidesByMonthAsync(date.Year, date.Month);
                RidesGrid.ItemsSource = rides;
                StatusBarText.Text = $"Отфильтровано по месяцу: {date:MMMM yyyy}";
                FilterDatePicker.SelectedDate = null;
            }
        }

        private async void ClearMonthFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadRides();
            MonthFilterPicker.SelectedDate = null;
            FilterDatePicker.SelectedDate = null;
            StatusBarText.Text = "Показаны все заезды";
        }

        #endregion

        #region Обработчики участия

        private async void AddParticipantBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new ParticipantWindow(_api);
            if (window.ShowDialog() == true) { await LoadParticipants(); await LoadDashboardStats(); StatusBarText.Text = "Участник записан на заезд"; }
        }

        private async void DeleteParticipantBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ParticipantsGrid.SelectedItem is RideParticipant selected)
            {
                if (MessageBox.Show($"Удалить запись участника {selected.ClientName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    int rideId = selected.RideId;
                    await _api.DeleteRideParticipantAsync(selected.Id);
                    await _api.UpdateRideTotalAmountAsync(rideId);
                    await LoadParticipants();
                    await LoadRides();
                    await LoadDashboardStats();
                    StatusBarText.Text = "Запись удалена";
                }
            }
            else MessageBox.Show("Выберите запись для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RefreshParticipantsBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadParticipants();
            StatusBarText.Text = "Список участников обновлён";
        }

        #endregion

        #region Обработчики запчастей

        private async void AddPartBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new PartWindow(_api);
            if (window.ShowDialog() == true) { await LoadParts(); StatusBarText.Text = "Список запчастей обновлён"; }
        }

        private async void EditPartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PartsGrid.SelectedItem is PartDTO selected)
            {
                var window = new PartWindow(_api, selected);
                if (window.ShowDialog() == true) { await LoadParts(); StatusBarText.Text = "Запчасть обновлена"; }
            }
            else MessageBox.Show("Выберите запчасть для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void DeletePartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PartsGrid.SelectedItem is PartDTO selected)
            {
                if (MessageBox.Show($"Удалить запчасть {selected.NameParts}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await _api.DeletePartAsync(selected.Id);
                    await LoadParts();
                    StatusBarText.Text = "Запчасть удалена";
                }
            }
            else MessageBox.Show("Выберите запчасть для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RefreshPartsBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadParts();
            StatusBarText.Text = "Список запчастей обновлён";
        }

        #endregion

        #region Обработчики ТО

        private async void AddTOBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new TOWindow(_api);
            if (window.ShowDialog() == true) { await LoadTO(); StatusBarText.Text = "Список ТО обновлён"; }
        }

        private async void EditTOBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TOGrid.SelectedItem is TODTO selected)
            {
                var window = new TOWindow(_api, selected);
                if (window.ShowDialog() == true) { await LoadTO(); StatusBarText.Text = "Запись ТО обновлена"; }
            }
            else MessageBox.Show("Выберите запись ТО для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void DeleteTOBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TOGrid.SelectedItem is TODTO selected)
            {
                if (MessageBox.Show($"Удалить запись ТО #{selected.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await _api.DeleteTOAsync(selected.Id);
                    await LoadTO();
                    StatusBarText.Text = "Запись ТО удалена";
                }
            }
            else MessageBox.Show("Выберите запись ТО для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RefreshTOBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadTO();
            StatusBarText.Text = "Список ТО обновлён";
        }

        #endregion

        #region Обработчики типов ТО

        private async void AddTypeTOBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new TypeTOWindow(_api);
            if (window.ShowDialog() == true) { await LoadTypeTO(); StatusBarText.Text = "Список типов ТО обновлён"; }
        }

        private async void EditTypeTOBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TypeTOGrid.SelectedItem is TypeTODTO selected)
            {
                var window = new TypeTOWindow(_api, selected);
                if (window.ShowDialog() == true) { await LoadTypeTO(); StatusBarText.Text = "Тип ТО обновлён"; }
            }
            else MessageBox.Show("Выберите тип ТО для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void DeleteTypeTOBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TypeTOGrid.SelectedItem is TypeTODTO selected)
            {
                if (MessageBox.Show($"Удалить тип ТО {selected.NameType}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await _api.DeleteTypeTOAsync(selected.Id);
                    await LoadTypeTO();
                    StatusBarText.Text = "Тип ТО удалён";
                }
            }
            else MessageBox.Show("Выберите тип ТО для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RefreshTypeTOBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadTypeTO();
            StatusBarText.Text = "Список типов ТО обновлён";
        }

        #endregion

        #region Обработчики сотрудников

        private async void AddPersonalBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new PersonalWindow(_api);
            if (window.ShowDialog() == true) { await LoadPersonal(); StatusBarText.Text = "Список сотрудников обновлён"; }
        }

        private async void EditPersonalBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PersonalGrid.SelectedItem is PersonalDTO selected)
            {
                var window = new PersonalWindow(_api, selected);
                if (window.ShowDialog() == true) { await LoadPersonal(); StatusBarText.Text = "Сотрудник обновлён"; }
            }
            else MessageBox.Show("Выберите сотрудника для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void DeletePersonalBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PersonalGrid.SelectedItem is PersonalDTO selected)
            {
                if (MessageBox.Show($"Удалить сотрудника {selected.FullName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await _api.DeletePersonalAsync(selected.Id);
                    await LoadPersonal();
                    StatusBarText.Text = "Сотрудник удалён";
                }
            }
            else MessageBox.Show("Выберите сотрудника для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RefreshPersonalBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadPersonal();
            StatusBarText.Text = "Список сотрудников обновлён";
        }

        #endregion

        #region Экспорт / Импорт Excel (только для владельца)

        private async void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            var user = App.Current.Properties["CurrentUser"] as UserInfoDTO;
            if (user == null || (user.Role?.ToLower() != "владелец" && user.Role?.ToLower() != "owner"))
            {
                MessageBox.Show("Экспорт доступен только владельцу.", "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveFileDialog.FileName = $"KartingCenter_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var clients = await _api.GetClientsAsync();
                    var teams = await _api.GetTeamsAsync();
                    var cartTypes = await _api.GetCartTypesAsync();
                    var karts = await _api.GetKartsAsync();
                    var locations = await _api.GetLocationsAsync();
                    var rides = await _api.GetRidesAsync();
                    var participants = await _api.GetRideParticipantsAsync();
                    var parts = await _api.GetPartsAsync();
                    var tos = await _api.GetTOAsync();
                    var typeTOs = await _api.GetTypeTOAsync();
                    var personal = await _api.GetPersonalAsync();

                    using (var package = new ExcelPackage())
                    {
                        AddSheet(package, "Clients", clients);
                        AddSheet(package, "Teams", teams);
                        AddSheet(package, "CartTypes", cartTypes);
                        AddSheet(package, "Karts", karts);
                        AddSheet(package, "Locations", locations);
                        AddSheet(package, "Rides", rides);
                        AddSheet(package, "RideParticipants", participants);
                        AddSheet(package, "Parts", parts);
                        AddSheet(package, "TO", tos);
                        AddSheet(package, "TypeTO", typeTOs);
                        AddSheet(package, "Personal", personal);

                        FileInfo fileInfo = new FileInfo(saveFileDialog.FileName);
                        package.SaveAs(fileInfo);
                    }

                    MessageBox.Show($"Данные успешно экспортированы в {saveFileDialog.FileName}", "Экспорт завершён", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddSheet<T>(ExcelPackage package, string sheetName, IEnumerable<T> data)
        {
            if (data == null || !data.Any()) return;
            var worksheet = package.Workbook.Worksheets.Add(sheetName);
            var properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = properties[i].Name;
            }
            int row = 2;
            foreach (var item in data)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(item);
                    worksheet.Cells[row, col + 1].Value = value?.ToString();
                }
                row++;
            }
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private async void ImportBtn_Click(object sender, RoutedEventArgs e)
        {
            var user = App.Current.Properties["CurrentUser"] as UserInfoDTO;
            if (user == null || (user.Role?.ToLower() != "владелец" && user.Role?.ToLower() != "owner"))
            {
                MessageBox.Show("Импорт доступен только владельцу.", "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var package = new ExcelPackage(new FileInfo(openFileDialog.FileName)))
                    {
                        var progress = new Progress<string>(msg => StatusBarText.Text = msg);
                        var result = await ImportAllData(package, progress);

                        if (result.Success)
                        {
                            await LoadAllData();
                            await LoadDashboardStats();
                            MessageBox.Show($"Импорт завершён успешно!\nСоздано: {result.CreatedCount} записей.",
                                "Импорт", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Импорт завершён с ошибками:\n{result.ErrorMessage}",
                                "Импорт", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка импорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task<ImportResult> ImportAllData(ExcelPackage package, IProgress<string> progress)
        {
            var result = new ImportResult();
            var createdCount = 0;
            var errors = new List<string>();

            var cartTypeMap = new Dictionary<string, int>();
            var locationMap = new Dictionary<string, int>();
            var teamMap = new Dictionary<string, int>();
            var clientMap = new Dictionary<string, int>();
            var kartMap = new Dictionary<string, int>();

            try
            {
                progress.Report("Импорт типов картов...");
                var cartTypes = ReadSheet<CartType>(package, "CartTypes");
                foreach (var ct in cartTypes)
                {
                    if (!await _api.CreateCartTypeIfNotExists(ct, cartTypeMap))
                    {
                        errors.Add($"Не удалось создать тип карта: {ct.Name}");
                    }
                    createdCount++;
                }

                progress.Report("Импорт локаций...");
                var locations = ReadSheet<Location>(package, "Locations");
                foreach (var loc in locations)
                {
                    if (!await _api.CreateLocationIfNotExists(loc, locationMap))
                    {
                        errors.Add($"Не удалось создать локацию: {loc.Name}");
                    }
                    createdCount++;
                }

                progress.Report("Импорт команд...");
                var teams = ReadSheet<Team>(package, "Teams");
                foreach (var team in teams)
                {
                    if (!await _api.CreateTeamIfNotExists(team, teamMap))
                    {
                        errors.Add($"Не удалось создать команду: {team.Name}");
                    }
                    createdCount++;
                }

                progress.Report("Импорт клиентов...");
                var clients = ReadSheet<Client>(package, "Clients");
                foreach (var client in clients)
                {
                    if (!await _api.CreateClientIfNotExists(client, clientMap))
                    {
                        errors.Add($"Не удалось создать клиента: {client.FullName}");
                    }
                    createdCount++;
                }

                progress.Report("Импорт картов...");
                var karts = ReadSheet<Kart>(package, "Karts");
                foreach (var kart in karts)
                {
                    if (!cartTypeMap.TryGetValue(kart.CartTypeName, out int cartTypeId))
                    {
                        errors.Add($"Тип карта не найден: {kart.CartTypeName}");
                        continue;
                    }
                    kart.CartTypeId = cartTypeId;
                    if (!await _api.CreateKartIfNotExists(kart, kartMap))
                    {
                        errors.Add($"Не удалось создать карт: {kart.SerialNumber}");
                    }
                    createdCount++;
                }

                progress.Report("Импорт заездов...");
                var rides = ReadSheet<Ride>(package, "Rides");
                foreach (var ride in rides)
                {
                    if (!locationMap.TryGetValue(ride.LocationName, out int locationId))
                    {
                        errors.Add($"Локация не найдена: {ride.LocationName}");
                        continue;
                    }
                    ride.LocationId = locationId;
                    if (!await _api.CreateRideIfNotExists(ride))
                    {
                        errors.Add($"Не удалось создать заезд: {ride.RideDate} {ride.StartTime}");
                    }
                    createdCount++;
                }


                result.Success = errors.Count == 0;
                result.CreatedCount = createdCount;
                result.ErrorMessage = string.Join("\n", errors);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private List<T> ReadSheet<T>(ExcelPackage package, string sheetName) where T : new()
        {
            var worksheet = package.Workbook.Worksheets[sheetName];
            if (worksheet == null) return new List<T>();

            var properties = typeof(T).GetProperties();
            var result = new List<T>();
            var rowCount = worksheet.Dimension.Rows;
            var colCount = worksheet.Dimension.Columns;

            for (int row = 2; row <= rowCount; row++)
            {
                var item = new T();
                bool hasData = false;

                for (int col = 1; col <= Math.Min(colCount, properties.Length); col++)
                {
                    var value = worksheet.Cells[row, col].Value;
                    if (value != null)
                    {
                        hasData = true;
                        var prop = properties[col - 1];
                        var propType = prop.PropertyType;

                        try
                        {
                            if (propType == typeof(string))
                                prop.SetValue(item, value.ToString());
                            else if (propType == typeof(int))
                                prop.SetValue(item, Convert.ToInt32(value));
                            else if (propType == typeof(decimal))
                                prop.SetValue(item, Convert.ToDecimal(value));
                            else if (propType == typeof(float))
                                prop.SetValue(item, Convert.ToSingle(value));
                            else if (propType == typeof(bool))
                                prop.SetValue(item, Convert.ToBoolean(value));
                            else if (propType == typeof(DateTime))
                                prop.SetValue(item, Convert.ToDateTime(value));
                            else if (propType == typeof(TimeSpan))
                                prop.SetValue(item, TimeSpan.Parse(value.ToString()));
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Ошибка преобразования: {prop.Name} = {value} ({ex.Message})");
                        }
                    }
                }

                if (hasData)
                    result.Add(item);
            }

            return result;
        }

        private class ImportResult
        {
            public bool Success { get; set; }
            public int CreatedCount { get; set; }
            public string ErrorMessage { get; set; }
        }

        #endregion
    }
}