using System;
using System.Linq;
using System.Windows;
using KartingCenter.Models;
using KartingCenter.Services;

namespace KartingCenter.Windows
{
    public partial class ParticipantWindow : Window
    {
        private readonly ApiService _api;

        public ParticipantWindow(ApiService api)
        {
            InitializeComponent();
            _api = api;
            Title = "Запись на заезд";
            Loaded += async (s, e) => await LoadData();
        }

        private async System.Threading.Tasks.Task LoadData()
        {
            ClientCombo.SelectedIndex = -1;
            TeamCombo.SelectedIndex = -1;
            RideCombo.SelectedIndex = -1;
            KartCombo.SelectedIndex = -1;

            var clients = await _api.GetClientsAsync();
            ClientCombo.ItemsSource = clients;

            var teams = await _api.GetTeamsAsync();
            var activeTeams = teams.Where(t => t.IsActive).ToList();
            TeamCombo.ItemsSource = activeTeams;

            var rides = await _api.GetRidesAsync();
            RideCombo.ItemsSource = rides.Select(r => new
            {
                Id = r.Id,
                Display = $"Заезд #{r.Id} - {r.RideDate:dd.MM.yyyy} {r.StartTime}"
            });
            RideCombo.DisplayMemberPath = "Display";
            RideCombo.SelectedValuePath = "Id";

            var karts = await _api.GetKartsAsync();
            var activeKarts = karts.Where(k => k.IsActive).ToList();
            KartCombo.ItemsSource = activeKarts;
            KartCombo.DisplayMemberPath = "SerialNumber";
            KartCombo.SelectedValuePath = "Id";
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ClientCombo.SelectedValue == null || TeamCombo.SelectedValue == null ||
                RideCombo.SelectedValue == null || KartCombo.SelectedValue == null)
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var participant = new CreateRideParticipant
            {
                ClientId = (int)ClientCombo.SelectedValue,
                TeamId = (int)TeamCombo.SelectedValue,
                RideId = (int)RideCombo.SelectedValue,
                KartId = (int)KartCombo.SelectedValue
            };

            try
            {
                var result = await _api.CreateRideParticipantAsync(participant);
                if (result != null)
                {
                    await _api.UpdateRideTotalAmountAsync(participant.RideId);

                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при записи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}