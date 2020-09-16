using Client;
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
using DBConnection;
using System.Net.Sockets;

namespace SingIn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;
        public TcpClient client;
        public NetworkStream stream;
        public SignIn()
        {
            InitializeComponent();
        }
        private void ButtonAuthorization_Click(object sender, RoutedEventArgs e)
        {
            DBconnection dBconnection = new DBconnection();
            dBconnection.ConnectDB();
            if (dBconnection.IsConnect())
            {
                string query = $"SELECT DISTINCT * FROM Account WHERE login = '{InputLogin.Text}' AND pass = '{InputPassword.Password}';";
                var result = dBconnection.SelectQuery(query);
                if (result.Read())
                {
                    Messanger messanger = new Messanger(InputLogin.Text);
                    messanger.Show();
                    this.Close();
                }
                else
                {
                    LabelCheck.Foreground = Brushes.Red;
                    LabelCheck.Text = "Логин или пароль неверный";
                }
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            InputLogin.Clear();
            InputPassword.Clear();
            LabelCheck.Text = "";
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HyperSignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUp signUp = new SignUp();
            signUp.Show();
            this.Close();
        }

        private void HyperRestorePassword_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (InputLogin.Text != "" && InputPassword.Password != "")
                {
                    ButtonAuthorization_Click(sender, e);
                }
            }
        }
    }
}
