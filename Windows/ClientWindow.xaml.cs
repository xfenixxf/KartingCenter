using System;
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

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text) || string.IsNullOrWhiteSpace(PhoneBox.Text))
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var client = new Client
            {
                FullName = FullNameBox.Text,
                Phone = PhoneBox.Text,
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