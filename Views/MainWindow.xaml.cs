using KartingCenter.Models;
using KartingCenter.Services;
using KartingCenter.Windows;
using System;
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
        }

        private async Task LoadAllData()
        {
            await LoadClients();
            await LoadTeams();
            await LoadCartTypes();
            await LoadKarts();
            await LoadLocations();
            await LoadRides();
            await LoadParticipants();
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

        private async Task LoadDashboardStats()
        {
            var rides = await _api.GetRidesAsync();
            int todayRidesCount = rides.Count(r => r.RideDate.Date == DateTime.Today.Date);

            string stickerColor;
            string statusDescription;

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
                case "Зелёный":
                    color = Brushes.Green;
                    break;
                case "Жёлтый":
                    color = Brushes.Gold;
                    break;
                case "Красный":
                    color = Brushes.Red;
                    break;
            }
            StickerIndicator.Background = color;
        }

        private async void AddClientBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new ClientWindow(_api);
            if (window.ShowDialog() == true)
            {
                await LoadClients();
                StatusBarText.Text = "Список клиентов обновлён";
            }
        }

        private async void EditClientBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Client selected)
            {
                var window = new ClientWindow(_api, selected);
                if (window.ShowDialog() == true)
                {
                    await LoadClients();
                    StatusBarText.Text = "Клиент обновлён";
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteClientBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Client selected)
            {
                var result = MessageBox.Show($"Удалить {selected.FullName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await _api.DeleteClientAsync(selected.Id);
                    await LoadClients();
                    StatusBarText.Text = "Клиент удалён";
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void RefreshClientsBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadClients();
            StatusBarText.Text = "Список клиентов обновлён";
        }

        private async void AddTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new TeamWindow(_api);
            if (window.ShowDialog() == true)
            {
                await LoadTeams();
                StatusBarText.Text = "Список команд обновлён";
            }
        }

        private async void EditTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TeamsGrid.SelectedItem is Team selected)
            {
                var window = new TeamWindow(_api, selected);
                if (window.ShowDialog() == true)
                {
                    await LoadTeams();
                    StatusBarText.Text = "Команда обновлена";
                }
            }
            else
            {
                MessageBox.Show("Выберите команду для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TeamsGrid.SelectedItem is Team selected)
            {
                var result = MessageBox.Show($"Удалить команду {selected.Name}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await _api.DeleteTeamAsync(selected.Id);
                    await LoadTeams();
                    StatusBarText.Text = "Команда удалена";
                }
            }
            else
            {
                MessageBox.Show("Выберите команду для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void RefreshTeamsBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadTeams();
            StatusBarText.Text = "Список команд обновлён";
        }

        private async void AddCartTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new CartTypeWindow(_api);
            if (window.ShowDialog() == true)
            {
                await LoadCartTypes();
                StatusBarText.Text = "Список типов картов обновлён";
            }
        }

        private async void EditCartTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CartTypesGrid.SelectedItem is CartType selected)
            {
                var window = new CartTypeWindow(_api, selected);
                if (window.ShowDialog() == true)
                {
                    await LoadCartTypes();
                    StatusBarText.Text = "Тип карта обновлён";
                }
            }
            else
            {
                MessageBox.Show("Выберите тип карта для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteCartTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CartTypesGrid.SelectedItem is CartType selected)
            {
                var result = MessageBox.Show($"Удалить тип карта {selected.Name}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await _api.DeleteCartTypeAsync(selected.Id);
                    await LoadCartTypes();
                    StatusBarText.Text = "Тип карта удалён";
                }
            }
            else
            {
                MessageBox.Show("Выберите тип карта для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void RefreshCartTypesBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadCartTypes();
            StatusBarText.Text = "Список типов картов обновлён";
        }

        private async void AddKartBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new KartWindow(_api);
            if (window.ShowDialog() == true)
            {
                await LoadKarts();
                StatusBarText.Text = "Список картов обновлён";
            }
        }

        private async void EditKartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (KartsGrid.SelectedItem is Kart selected)
            {
                var window = new KartWindow(_api, selected);
                if (window.ShowDialog() == true)
                {
                    await LoadKarts();
                    StatusBarText.Text = "Карт обновлён";
                }
            }
            else
            {
                MessageBox.Show("Выберите карт для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteKartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (KartsGrid.SelectedItem is Kart selected)
            {
                var result = MessageBox.Show($"Удалить карт {selected.SerialNumber}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await _api.DeleteKartAsync(selected.Id);
                    await LoadKarts();
                    StatusBarText.Text = "Карт удалён";
                }
            }
            else
            {
                MessageBox.Show("Выберите карт для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void RefreshKartsBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadKarts();
            StatusBarText.Text = "Список картов обновлён";
        }

        private async void AddLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new LocationWindow(_api);
            if (window.ShowDialog() == true)
            {
                await LoadLocations();
                StatusBarText.Text = "Список локаций обновлён";
            }
        }

        private async void EditLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LocationsGrid.SelectedItem is Location selected)
            {
                var window = new LocationWindow(_api, selected);
                if (window.ShowDialog() == true)
                {
                    await LoadLocations();
                    StatusBarText.Text = "Локация обновлена";
                }
            }
            else
            {
                MessageBox.Show("Выберите локацию для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LocationsGrid.SelectedItem is Location selected)
            {
                var result = MessageBox.Show($"Удалить локацию {selected.Name}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await _api.DeleteLocationAsync(selected.Id);
                    await LoadLocations();
                    StatusBarText.Text = "Локация удалена";
                }
            }
            else
            {
                MessageBox.Show("Выберите локацию для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void RefreshLocationsBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadLocations();
            StatusBarText.Text = "Список локаций обновлён";
        }

        private async void AddRideBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new RideWindow(_api);
            if (window.ShowDialog() == true)
            {
                await LoadRides();
                await LoadDashboardStats();
                StatusBarText.Text = "Список заездов обновлён";
            }
        }

        private async void EditRideBtn_Click(object sender, RoutedEventArgs e)
        {
            if (RidesGrid.SelectedItem is Ride selected)
            {
                var window = new RideWindow(_api, selected);
                if (window.ShowDialog() == true)
                {
                    await LoadRides();
                    await LoadDashboardStats();
                    StatusBarText.Text = "Заезд обновлён";
                }
            }
            else
            {
                MessageBox.Show("Выберите заезд для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteRideBtn_Click(object sender, RoutedEventArgs e)
        {
            if (RidesGrid.SelectedItem is Ride selected)
            {
                var result = MessageBox.Show($"Удалить заезд #{selected.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await _api.DeleteRideAsync(selected.Id);
                    await LoadRides();
                    await LoadDashboardStats();
                    StatusBarText.Text = "Заезд удалён";
                }
            }
            else
            {
                MessageBox.Show("Выберите заезд для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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

        private async void AddParticipantBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new ParticipantWindow(_api);
            if (window.ShowDialog() == true)
            {
                await LoadParticipants();
                await LoadDashboardStats();
                StatusBarText.Text = "Участник записан на заезд";
            }
        }

        private async void DeleteParticipantBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ParticipantsGrid.SelectedItem is RideParticipant selected)
            {
                var result = MessageBox.Show($"Удалить запись участника {selected.ClientName}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
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
            else
            {
                MessageBox.Show("Выберите запись для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void RefreshParticipantsBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadParticipants();
            StatusBarText.Text = "Список участников обновлён";
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
    }
}