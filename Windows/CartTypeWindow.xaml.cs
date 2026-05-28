using System;
using System.Windows;
using KartingCenter.Models;
using KartingCenter.Services;

namespace KartingCenter.Windows
{
    public partial class CartTypeWindow : Window
    {
        private readonly ApiService _api;
        private readonly CartType _editingCartType;
        private readonly bool _isEditMode;

        public CartTypeWindow(ApiService api, CartType cartType = null)
        {
            InitializeComponent();
            _api = api;
            _editingCartType = cartType;
            _isEditMode = cartType != null;

            if (_isEditMode)
            {
                Title = "Редактирование типа карта";
                NameBox.Text = cartType.Name;
                PriceBox.Text = cartType.PricePerRace.ToString();
                IsAvailableBox.IsChecked = cartType.IsAvailable;
            }
            else
            {
                Title = "Добавление типа карта";
                IsAvailableBox.IsChecked = true;
            }
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) || string.IsNullOrWhiteSpace(PriceBox.Text))
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Цена должна быть положительным числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var cartType = new CartType
            {
                Name = NameBox.Text,
                PricePerRace = price,
                IsAvailable = IsAvailableBox.IsChecked ?? true
            };

            bool success;
            if (_isEditMode)
            {
                cartType.Id = _editingCartType.Id;
                success = await _api.UpdateCartTypeAsync(cartType.Id, cartType);
                
            }
            else
            {
                var result = await _api.CreateCartTypeAsync(cartType);
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