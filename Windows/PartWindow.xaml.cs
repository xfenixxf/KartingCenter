using System;
using System.Windows;
using KartingCenter.Models;
using KartingCenter.Services;

namespace KartingCenter.Windows
{
    public partial class PartWindow : Window
    {
        private readonly ApiService _api;
        private readonly PartDTO _editing;
        private readonly bool _isEdit;

        public PartWindow(ApiService api, PartDTO item = null)
        {
            InitializeComponent();
            _api = api;
            _editing = item;
            _isEdit = item != null;
            Title = _isEdit ? "Редактирование запчасти" : "Добавление запчасти";

            if (_isEdit)
            {
                NameBox.Text = item.NameParts;
                DescriptionBox.Text = item.Description;
                QuantityBox.Text = item.Quantity.ToString();
                PriceBox.Text = item.Price.ToString();
            }
        }

        private bool Validate(out string msg)
        {
            msg = "";
            if (string.IsNullOrWhiteSpace(NameBox.Text)) { msg = "Введите название"; return false; }
            if (!int.TryParse(QuantityBox.Text, out int q) || q < 0)
            { msg = "Количество должно быть целым неотрицательным числом"; return false; }
            if (!float.TryParse(PriceBox.Text, out float p) || p < 0)
            { msg = "Цена должна быть положительным числом"; return false; }
            return true;
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate(out string err))
            {
                MessageBox.Show(err, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dto = new CreatePartDTO
            {
                NameParts = NameBox.Text.Trim(),
                Description = DescriptionBox.Text?.Trim(),
                Quantity = int.Parse(QuantityBox.Text),
                Price = float.Parse(PriceBox.Text)
            };

            bool success;
            if (_isEdit)
                success = await _api.UpdatePartAsync(_editing.Id, dto);
            else
                success = await _api.CreatePartAsync(dto) != null;

            if (success)
            {
                DialogResult = true;
                Close();
            }
            else
                MessageBox.Show("Ошибка сохранения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}