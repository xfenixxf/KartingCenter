
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

        private bool ValidateData(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                errorMessage = "Название типа карта обязательно для заполнения";
                return false;
            }

            if (NameBox.Text.Length < 2 || NameBox.Text.Length > 100)
            {
                errorMessage = "Название должно содержать от 2 до 100 символов";
                return false;
            }

            if (string.IsNullOrWhiteSpace(PriceBox.Text))
            {
                errorMessage = "Цена за заезд обязательна для заполнения";
                return false;
            }

            if (!decimal.TryParse(PriceBox.Text, out decimal price))
            {
                errorMessage = "Введите корректное числовое значение цены";
                return false;
            }

            if (price <= 0)
            {
                errorMessage = "Цена должна быть положительным числом";
                return false;
            }

            if (price > 1000000)
            {
                errorMessage = "Цена не может превышать 1 000 000 рублей";
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

            var cartType = new CartType
            {
                Name = NameBox.Text.Trim(),
                PricePerRace = decimal.Parse(PriceBox.Text),
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