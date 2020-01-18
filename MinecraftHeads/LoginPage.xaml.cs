using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MinecraftHeads
{
    /// <summary>
    /// Interakční logika pro LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        private void login()
        {
            if (App.APIHandlerObject.Login(LoginField.Text, PasswordField.Password) != null)
            {
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(App.MainPageObject);
                App.MainPageObject.UpdatePage();
            }
            else
            {
                MessageLabel.Text = "Login failed.\nPlease check your credentials or try again later.";
                MessageLabel.Foreground = Brushes.Red;
            }
        }
        private void loginButtonPress(object sender, RoutedEventArgs e)
        {
            login();
        }
        private void textBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                login();
            }
        }
    }
}
