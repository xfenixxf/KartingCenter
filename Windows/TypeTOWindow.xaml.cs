using System;
using System.Windows;
using KartingCenter.Models;
using KartingCenter.Services;

namespace KartingCenter.Windows
{
    public partial class TypeTOWindow : Window
    {
        private readonly ApiService _api;
        private readonly TypeTODTO _editing;
        private readonly bool _isEdit;

        public TypeTOWindow(ApiService api, TypeTODTO item = null)
        {
            InitializeComponent();
            _api = api;
            _editing = item;
            _isEdit = item != null;
            Title = _isEdit ? "Редактирование типа ТО" : "Добавление типа ТО";

            if (_isEdit)
            {
                NameBox.Text = item.NameType;
                DescriptionBox.Text = item.Description;
                IntervalBox.Text = item.RecommendedInterval?.ToString() ?? "";
            }
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название типа ТО", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            float? interval = null;
            if (!string.IsNullOrWhiteSpace(IntervalBox.Text))
            {
                if (!float.TryParse(IntervalBox.Text, out float val))
                {
                    MessageBox.Show("Интервал должен быть числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                interval = val;
            }

            var dto = new CreateTypeTODTO
            {
                NameType = NameBox.Text.Trim(),
                Description = DescriptionBox.Text?.Trim(),
                RecommendedInterval = interval
            };

            bool success = false;
            if (_isEdit)
                success = await _api.UpdateTypeTOAsync(_editing.Id, dto);
            else
                success = await _api.CreateTypeTOAsync(dto) != null;

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