using System;
using System.Linq;
using System.Windows;
using KartingCenter.Models;
using KartingCenter.Services;

namespace KartingCenter.Windows
{
    public partial class KartWindow : Window
    {
        private readonly ApiService _api;
        private readonly Kart _editingKart;
        private readonly bool _isEditMode;

        public KartWindow(ApiService api, Kart kart = null)
        {
            InitializeComponent();
            _api = api;
            _editingKart = kart;
            _isEditMode = kart != null;

            LoadCartTypes();

            if (_isEditMode)
            {
                Title = "Редактирование карта";
                SerialNumberBox.Text = kart.SerialNumber;
                CartTypeCombo.SelectedValue = kart.CartTypeId;
                PriceBox.Text = kart.Price.ToString();
                IsActiveBox.IsChecked = kart.IsActive;
            }
            else
            {
                Title = "Добавление карта";
                IsActiveBox.IsChecked = true;
            }
        }

        private async void LoadCartTypes()
        {
            var cartTypes = await _api.GetCartTypesAsync();
            CartTypeCombo.ItemsSource = cartTypes;
            if (cartTypes.Any())
                CartTypeCombo.SelectedIndex = 0;
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SerialNumberBox.Text) || CartTypeCombo.SelectedValue == null)
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Цена должна быть положительным числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var kart = new Kart
            {
                SerialNumber = SerialNumberBox.Text,
                CartTypeId = (int)CartTypeCombo.SelectedValue,
                Price = price,
                IsActive = IsActiveBox.IsChecked ?? true
            };

            bool success;
            if (_isEditMode)
            {
                kart.Id = _editingKart.Id;
                success = await _api.UpdateKartAsync(kart.Id, kart);
               
            }
            else
            {
                var result = await _api.CreateKartAsync(kart);
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