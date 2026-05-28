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

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название команды", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var team = new Team
            {
                Name = NameBox.Text,
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