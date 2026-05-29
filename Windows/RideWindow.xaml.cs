// RideWindow.xaml.cs
using System;
using System.Linq;
using System.Windows;
using KartingCenter.Models;
using KartingCenter.Services;

namespace KartingCenter.Windows
{
    public partial class RideWindow : Window
    {
        private readonly ApiService _api;
        private readonly Ride _editingRide;
        private readonly bool _isEditMode;

        public RideWindow(ApiService api, Ride ride = null)
        {
            InitializeComponent();
            _api = api;
            _editingRide = ride;
            _isEditMode = ride != null;

            LoadLocations();

            if (_isEditMode)
            {
                Title = "Редактирование заезда";
                RideDatePicker.SelectedDate = ride.RideDate;
                StartTimeBox.Text = ride.StartTime.ToString();
                LocationCombo.SelectedValue = ride.LocationId;
                AmountBox.Text = ride.AmountPaid.ToString("F2");
            }
            else
            {
                Title = "Добавление заезда";
                RideDatePicker.SelectedDate = DateTime.Today;
                StartTimeBox.Text = "10:00:00";
                AmountBox.Text = "0.00";
            }
        }

        private async void LoadLocations()
        {
            var locations = await _api.GetLocationsAsync();
            LocationCombo.ItemsSource = locations;
            if (locations.Any())
                LocationCombo.SelectedIndex = 0;
        }

        private bool ValidateData(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (RideDatePicker.SelectedDate == null)
            {
                errorMessage = "Выберите дату заезда";
                return false;
            }

            if (RideDatePicker.SelectedDate.Value.Date < DateTime.Today.Date)
            {
                errorMessage = "Дата заезда не может быть в прошлом";
                return false;
            }

            if (string.IsNullOrWhiteSpace(StartTimeBox.Text))
            {
                errorMessage = "Введите время старта";
                return false;
            }

            if (!TimeSpan.TryParse(StartTimeBox.Text, out TimeSpan startTime))
            {
                errorMessage = "Введите корректное время в формате ЧЧ:ММ:СС";
                return false;
            }

            if (LocationCombo.SelectedValue == null)
            {
                errorMessage = "Выберите локацию";
                return false;
            }

            return true;
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TimeSpan.TryParse(StartTimeBox.Text, out TimeSpan startTime))
            {
                MessageBox.Show("Введите корректное время (ЧЧ:ММ:СС)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal amount;
            if (_isEditMode)
            {
                amount = _editingRide.AmountPaid;
            }
            else
            {
                amount = 0;
            }

            var ride = new Ride
            {
                LocationId = (int)LocationCombo.SelectedValue,
                RideDate = RideDatePicker.SelectedDate.Value,
                StartTime = startTime,
                AmountPaid = amount,
                RideData = ""
            };

            bool success;
            if (_isEditMode)
            {
                ride.Id = _editingRide.Id;
                success = await _api.UpdateRideAsync(ride.Id, ride);
            }
            else
            {
                var result = await _api.CreateRideAsync(ride);
                success = result != null;
            }

            if (success)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}