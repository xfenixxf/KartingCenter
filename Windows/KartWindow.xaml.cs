// KartWindow.xaml.cs
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

        private bool ValidateData(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(SerialNumberBox.Text))
            {
                errorMessage = "Серийный номер обязателен для заполнения";
                return false;
            }

            if (SerialNumberBox.Text.Length < 3 || SerialNumberBox.Text.Length > 50)
            {
                errorMessage = "Серийный номер должен содержать от 3 до 50 символов";
                return false;
            }

            if (CartTypeCombo.SelectedValue == null)
            {
                errorMessage = "Выберите тип карта";
                return false;
            }

            if (string.IsNullOrWhiteSpace(PriceBox.Text))
            {
                errorMessage = "Цена обязательна для заполнения";
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

            var kart = new Kart
            {
                SerialNumber = SerialNumberBox.Text.Trim(),
                CartTypeId = (int)CartTypeCombo.SelectedValue,
                Price = decimal.Parse(PriceBox.Text),
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