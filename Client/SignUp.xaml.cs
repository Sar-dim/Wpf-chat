using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DBConnection;
using System.Timers;
using System.Windows.Threading;
using System.Threading.Tasks;
using PasswordChecking;

namespace SingIn
{
    /// <summary>
    /// Логика взаимодействия для SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        PasswordCheck PasswordCheck = new PasswordCheck();
        public SignUp()
        {
            InitializeComponent();
        }

        private void ButtonSignUp_Click(object sender, RoutedEventArgs e)
        {
            DBconnection dBconnection = new DBconnection();
            dBconnection.ConnectDB();
            if (dBconnection.IsConnect())
            {
                string query = $"INSERT INTO Account(login, pass, status) " +
                    $"VALUES ('{InputLogin.Text}', '{InputPassword.Password}', 'I am tomato');";
                dBconnection.InsertQuery(query);
            }
            dBconnection.Close();
            SignIn signIn = new SignIn();
            signIn.Show();
            this.Close();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            InputLogin.Clear();
            InputPassword.Clear();
            InputRepeatPassword.Clear();
            LabelPasswordCheckAlphabet.Text = "";
            LabelPasswordCheckSymbols.Text = "";
            LabelPasswordCheckLength.Text = "";
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            SignIn signIn = new SignIn();
            signIn.Show();
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void InputPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordCheck.Success += (string message) => {
                LabelPasswordCheckSymbols.Foreground = Brushes.Green;
                LabelPasswordCheckSymbols.Text = message; 
            };
            PasswordCheck.Error += (string message) => {
                LabelPasswordCheckSymbols.Foreground = Brushes.Red;
                LabelPasswordCheckSymbols.Text = message;
            };
            PasswordCheck.CheckSymbol(InputPassword.Password);
            PasswordCheck.ClearEvents();
            PasswordCheck.Success += (string message) =>
            {
                LabelPasswordCheckAlphabet.Foreground = Brushes.Green;
                LabelPasswordCheckAlphabet.Text = message;
            };
            PasswordCheck.Error += (string message) =>
            {
                LabelPasswordCheckAlphabet.Foreground = Brushes.Red;
                LabelPasswordCheckAlphabet.Text = message;
            };
            PasswordCheck.CheckAlphabet(InputPassword.Password);
            PasswordCheck.ClearEvents();
            PasswordCheck.Success += (string message) =>
            {
                LabelPasswordCheckLength.Foreground = Brushes.Green;
                LabelPasswordCheckLength.Text = message;
            };
            PasswordCheck.Error += (string message) =>
            {
                LabelPasswordCheckLength.Foreground = Brushes.Red;
                LabelPasswordCheckLength.Text = message;
            };
            PasswordCheck.CheckLength(InputPassword.Password);
            PasswordCheck.ClearEvents();
            if (LabelPasswordCheckAlphabet.Foreground == Brushes.Green && LabelPasswordCheckSymbols.Foreground == Brushes.Green
                && LabelPasswordCheckLength.Foreground == Brushes.Green && LabelLoginCheck.Foreground == Brushes.Green)
            {
                InputRepeatPassword.IsEnabled = true;
            }
            else
            {
                InputRepeatPassword.IsEnabled = false;
            }
        }
        
        private void InputRepeatPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (InputPassword.Password == InputRepeatPassword.Password)
            {
                ButtonSignUp.IsEnabled = true;
            }
            else
            {
                ButtonSignUp.IsEnabled = false;
            }
        }
        private async void InputLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (await InputLogin.GetIdle())
            {
                DBconnection dBconnection = new DBconnection();
                dBconnection.ConnectDB();
                string query = $"SELECT * FROM Account WHERE login = '{InputLogin.Text.Trim()}';";
                var result = dBconnection.SelectQuery(query);
                if (result.Read())
                {
                    LabelLoginCheck.Foreground = Brushes.Red;
                    LabelLoginCheck.Text = "Логин занят";
                }
                else
                {
                    LabelLoginCheck.Foreground = Brushes.Green;
                    LabelLoginCheck.Text = "Логин свободен";
                }
            }
        }
        
    }
    public static class UIExtensionMethods
    {
        public static async Task<bool> GetIdle(this TextBox txb)
        {
            string txt = txb.Text;
            await Task.Delay(500);
            return txt == txb.Text;
        }
    }
}
