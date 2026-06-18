using System;
using System.Windows;
using KartingCenter.Models;
using KartingCenter.Services;

namespace KartingCenter.Windows
{
    public partial class PersonalWindow : Window
    {
        private readonly ApiService _api;
        private readonly PersonalDTO _editing;
        private readonly bool _isEdit;

        public PersonalWindow(ApiService api, PersonalDTO item = null)
        {
            InitializeComponent();
            _api = api;
            _editing = item;
            _isEdit = item != null;
            Title = _isEdit ? "Редактирование сотрудника" : "Добавление сотрудника";
            Loaded += async (s, e) => await LoadData();
        }

        private async System.Threading.Tasks.Task LoadData()
        {
            var roles = await _api.GetRolesAsync();
            RoleCombo.ItemsSource = roles;

            if (_isEdit)
            {
                FullNameBox.Text = _editing.FullName;
                LoginBox.Text = _editing.Email;
                LoginBox.IsEnabled = false;
                PasswordBox.Visibility = Visibility.Collapsed;
                RoleCombo.SelectedValue = _editing.RoleId;
            }
            else
            {
                LoginBox.Text = "";
                LoginBox.IsEnabled = true;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordBox.Password = "";
            }
        }

        private bool Validate(out string msg)
        {
            msg = "";
            if (string.IsNullOrWhiteSpace(FullNameBox.Text)) { msg = "Введите ФИО"; return false; }
            if (string.IsNullOrWhiteSpace(LoginBox.Text)) { msg = "Введите логин (email)"; return false; }
            if (!_isEdit && string.IsNullOrWhiteSpace(PasswordBox.Password)) { msg = "Введите пароль"; return false; }
            if (RoleCombo.SelectedValue == null) { msg = "Выберите роль"; return false; }
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
                var dto = new CreatePersonalDTO
                {
                    FullName = FullNameBox.Text.Trim(),
                    LoginId = _editing.LoginId,
                    RoleId = (int)RoleCombo.SelectedValue
                };
                bool success = await _api.UpdatePersonalAsync(_editing.Id, dto);
                if (success) { DialogResult = true; Close(); }
                else MessageBox.Show("Ошибка обновления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string password = PasswordBox.Password.Trim();
                if (string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Пароль не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var registerResult = await _api.RegisterAsync(LoginBox.Text.Trim(), password);
                if (registerResult != null && registerResult.Success)
                {
                    var dto = new CreatePersonalDTO
                    {
                        FullName = FullNameBox.Text.Trim(),
                        LoginId = registerResult.UserId,
                        RoleId = (int)RoleCombo.SelectedValue
                    };
                    var result = await _api.CreatePersonalAsync(dto);
                    if (result != null) { DialogResult = true; Close(); }
                    else MessageBox.Show("Ошибка создания сотрудника", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Ошибка создания пользователя: " + registerResult?.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}