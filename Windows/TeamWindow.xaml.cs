
using System.Windows;
using KartingCenter.Models;
using KartingCenter.Services;

namespace KartingCenter.Windows
{
    public partial class TeamWindow : Window
    {
        private readonly ApiService _api;
        private readonly Team _editingTeam;
        private readonly bool _isEditMode;

        public TeamWindow(ApiService api, Team team = null)
        {
            InitializeComponent();
            _api = api;
            _editingTeam = team;
            _isEditMode = team != null;

            if (_isEditMode)
            {
                Title = "Редактирование команды";
                NameBox.Text = team.Name;
                IsActiveBox.IsChecked = team.IsActive;
            }
            else
            {
                Title = "Добавление команды";
                IsActiveBox.IsChecked = true;
            }
        }

        private bool ValidateData(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                errorMessage = "Название команды обязательно для заполнения";
                return false;
            }

            if (NameBox.Text.Length < 2 || NameBox.Text.Length > 200)
            {
                errorMessage = "Название команды должно содержать от 2 до 200 символов";
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

            var team = new Team
            {
                Name = NameBox.Text.Trim(),
                IsActive = IsActiveBox.IsChecked ?? true
            };

            bool success;
            if (_isEditMode)
            {
                team.Id = _editingTeam.Id;
                success = await _api.UpdateTeamAsync(team.Id, team);
            }
            else
            {
                var result = await _api.CreateTeamAsync(team);
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