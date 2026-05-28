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
                Title = "✏️ Редактирование локации";
                NameBox.Text = location.Name;
                AddressBox.Text = location.Address;
            }
            else
            {
                Title = "➕ Добавление локации";
            }
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название локации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var location = new Location
            {
                Name = NameBox.Text,
                Address = AddressBox.Text
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