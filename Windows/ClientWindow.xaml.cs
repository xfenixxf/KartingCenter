
using System;
using System.Text.RegularExpressions;
using System.Windows;
using KartingCenter.Models;
using KartingCenter.Services;

namespace KartingCenter.Windows
{
    public partial class ClientWindow : Window
    {
        private readonly ApiService _api;
        private readonly Client _editingClient;
        private readonly bool _isEditMode;

        public ClientWindow(ApiService api, Client client = null)
        {
            InitializeComponent();
            _api = api;
            _editingClient = client;
            _isEditMode = client != null;

            if (_isEditMode)
            {
                Title = "Редактирование клиента";
                FullNameBox.Text = client.FullName;
                PhoneBox.Text = client.Phone;
                IsPermanentBox.IsChecked = client.IsPermanent;
            }
            else
            {
                Title = "Добавление клиента";
            }
        }

        private bool ValidateData(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                errorMessage = "ФИО обязательно для заполнения";
                return false;
            }

            if (FullNameBox.Text.Length < 2 || FullNameBox.Text.Length > 200)
            {
                errorMessage = "ФИО должно содержать от 2 до 200 символов";
                return false;
            }

            if (string.IsNullOrWhiteSpace(PhoneBox.Text))
            {
                errorMessage = "Телефон обязателен для заполнения";
                return false;
            }

            string phonePattern = @"^[\+]?[0-9\s\-\(\)]{10,20}$";
            if (!Regex.IsMatch(PhoneBox.Text, phonePattern))
            {
                errorMessage = "Введите корректный номер телефона";
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

            var client = new Client
            {
                FullName = FullNameBox.Text.Trim(),
                Phone = PhoneBox.Text.Trim(),
                IsPermanent = IsPermanentBox.IsChecked ?? false
            };

            bool success;
            if (_isEditMode)
            {
                client.Id = _editingClient.Id;
                success = await _api.UpdateClientAsync(client.Id, client);
            }
            else
            {
                var result = await _api.CreateClientAsync(client);
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