using System;
using System.Linq;
using System.Windows;
using KartingCenter.Models;
using KartingCenter.Services;

namespace KartingCenter.Windows
{
    public partial class TOWindow : Window
    {
        private readonly ApiService _api;
        private readonly TODTO _editing;
        private readonly bool _isEdit;

        public TOWindow(ApiService api, TODTO item = null)
        {
            InitializeComponent();
            _api = api;
            _editing = item;
            _isEdit = item != null;
            Title = _isEdit ? "Редактирование ТО" : "Добавление ТО";
            Loaded += async (s, e) => await LoadData();
        }

        private async System.Threading.Tasks.Task LoadData()
        {
            var karts = await _api.GetKartsAsync();
            KartCombo.ItemsSource = karts;
            var types = await _api.GetTypeTOAsync();
            TypeTOCombo.ItemsSource = types;
            var masters = await _api.GetPersonalAsync();
            MasterCombo.ItemsSource = masters;

            if (_isEdit)
            {
                KartCombo.SelectedValue = _editing.KartId;
                TypeTOCombo.SelectedValue = _editing.TypeTOId;
                MasterCombo.SelectedValue = _editing.MasterId;
                DatePicker.SelectedDate = _editing.Date;
                DescriptionBox.Text = _editing.Description;
                CompletedCheck.IsChecked = _editing.IsCompleted;
            }
            else
            {
                DatePicker.SelectedDate = DateTime.Today;
            }
        }

        private bool Validate(out string msg)
        {
            msg = "";
            if (KartCombo.SelectedValue == null) { msg = "Выберите карт"; return false; }
            if (TypeTOCombo.SelectedValue == null) { msg = "Выберите тип ТО"; return false; }
            if (MasterCombo.SelectedValue == null) { msg = "Выберите мастера"; return false; }
            if (DatePicker.SelectedDate == null) { msg = "Выберите дату"; return false; }
            return true;
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate(out string err))
            {
                MessageBox.Show(err, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEdit)
            {
                var dto = new UpdateTODTO
                {
                    TypeTOId = (int)TypeTOCombo.SelectedValue,
                    MasterId = (int)MasterCombo.SelectedValue,
                    Date = DatePicker.SelectedDate.Value,
                    Description = DescriptionBox.Text?.Trim(),
                    IsCompleted = CompletedCheck.IsChecked ?? false,
                    UsedParts = new System.Collections.Generic.List<CreateUsedPartDTO>()
                };
                bool ok = await _api.UpdateTOAsync(_editing.Id, dto);
                if (ok) { DialogResult = true; Close(); }
                else MessageBox.Show("Ошибка обновления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                var dto = new CreateTODTO
                {
                    KartId = (int)KartCombo.SelectedValue,
                    TypeTOId = (int)TypeTOCombo.SelectedValue,
                    MasterId = (int)MasterCombo.SelectedValue,
                    Date = DatePicker.SelectedDate.Value,
                    Description = DescriptionBox.Text?.Trim(),
                    UsedParts = new System.Collections.Generic.List<CreateUsedPartDTO>()
                };
                var result = await _api.CreateTOAsync(dto);
                if (result != null) { DialogResult = true; Close(); }
                else MessageBox.Show("Ошибка создания", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}