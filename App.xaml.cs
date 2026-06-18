using KartingCenter.Windows;
using OfficeOpenXml;
using System.Windows;

namespace KartingCenter
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Иван Иванов");
            var loginWindow = new LoginWindow();
            this.MainWindow = loginWindow;

            bool? result = loginWindow.ShowDialog();

            if (result == true)
            {
                var mainWindow = new MainWindow();
                this.MainWindow = mainWindow;
                mainWindow.Show();
            }
            else
            {
                Shutdown();
            }
        
       
        }
    }
}