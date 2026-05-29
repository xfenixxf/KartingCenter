
using System.Windows;
using KartingCenter.Models;
using KartingCenter.Services;

namespace KartingCenter.Windows
{
    public partial class LocationWindow : Window
    {
        private readonly ApiService _api;
        private readonly Location _editingLocation;
        private readonly bool _isEditMode;

        public LocationWindow(ApiService api, Location location = null)
        {
            InitializeComponent();
            _api = api;
            _editingLocation = location;
            _isEditMode = location != null;

            if (_isEditMode)
            {
                Title = "Редактирование локации";
                NameBox.Text = location.Name;
                AddressBox.Text = location.Address;
            }
            else
            {
                Title = "Добавление локации";
            }
        }

        private bool ValidateData(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                errorMessage = "Название локации обязательно для заполнения";
                return false;
            }

            if (NameBox.Text.Length < 2 || NameBox.Text.Length > 100)
            {
                errorMessage = "Название локации должно содержать от 2 до 100 символов";
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

            var location = new Location
            {
                Name = NameBox.Text.Trim(),
                Address = AddressBox.Text?.Trim()
            };

            bool success;
            if (_isEditMode)
            {
                location.Id = _editingLocation.Id;
                success = await _api.UpdateLocationAsync(location.Id, location);
            }
            else
            {
                var result = await _api.CreateLocationAsync(location);
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