using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using KartingCenter.Services;

namespace KartingCenter.Windows
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _api;

        public LoginWindow()
        {
            InitializeComponent();
            _api = new ApiService("http://localhost:55493/");
            LoginBox.Focus();
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoginAsync();
        }

        private async Task LoginAsync()
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowError("Введите логин и пароль!");
                return;
            }

            LoginBtn.IsEnabled = false;
            LoginBtn.Content = "Вход...";
            ErrorText.Visibility = Visibility.Collapsed;

            try
            {
                var result = await _api.LoginAsync(login, password);

                if (result != null && result.Success)
                {
                    App.Current.Properties["CurrentUser"] = result.User;
                    DialogResult = true;
                    Close();
                }
                else
                {
                    ShowError(result?.Message ?? "Неверный логин или пароль");
                    PasswordBox.Password = "";
                    PasswordBox.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка подключения к серверу: {ex.Message}");
            }
            finally
            {
                LoginBtn.IsEnabled = true;
                LoginBtn.Content = "Войти";
            }
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }

        private void LoginBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) PasswordBox.Focus();
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) LoginBtn_Click(sender, e);
        }
    }
}