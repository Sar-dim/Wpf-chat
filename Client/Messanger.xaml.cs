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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Messanger : Window
    {
        List<Contact> contacts = new List<Contact>();
        List<string> contactsString = new List<string>();
        int ContactListIndex = -1;
        public Messanger(string login)
        {
            InitializeComponent();
            DBconnection dBconnection = new DBconnection();
            dBconnection.ConnectDB();
            string query = $"SELECT DISTINCT * FROM Account WHERE login = '{login}'";
            var result = dBconnection.SelectQuery(query);
            if (result.Read())
            {
                string name = result.GetString(1);
                string status = result.GetString(4);
                MyContact.userName_textBlock.Text = name;
                MyContact.userTitle_textBlock.Text = status;
            }
            dBconnection.Close();

            query = $"SELECT * FROM Messages WHERE sender = '{login}' OR recepient = '{login}'";
            dBconnection.ConnectDB();
            result = dBconnection.SelectQuery(query);
            while (result.Read())
            {
                if (result.GetString(1) == login)
                {
                    if (!contactsString.Contains(result.GetString(2)))
                    {
                        contactsString.Add(result.GetString(2));
                    }
                }
                else if (result.GetString(2) == login)
                {
                    if (!contactsString.Contains(result.GetString(1)))
                    {
                        contactsString.Add(result.GetString(1));
                    }
                }
            }
            /*
            contacts.Add(new Contact(new List<Message>() { new Message("Hey dude", "13:00", true), new Message("Wazzzup", "13:01", false), new Message("How are u", "13:01", false) }, "Vasya", "Dear friend"));
            contacts.Add(new Contact(new List<Message>(), "Petya", "Colleague"));
            contacts.Add(new Contact(new List<Message>(), "Petya", "Colleague"));*/
            ContactList_listBox.ItemsSource = contacts; //обновление списка контактов на форме

        }

        private void search_textbox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (search_textbox.Text == "Searching")
            {
                search_textbox.Text = "";
            }
        }

        private void search_textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(search_textbox.Text))
            {
                search_textbox.Text = "Searching";
            }
        }

        private void sendFile_button1_Click(object sender, RoutedEventArgs e)
        {
        }

        private void sendSmile_button2_Click(object sender, RoutedEventArgs e)
        {
        }

        private void sendMessage_button3_Click(object sender, RoutedEventArgs e)
        {
        }

        private void sendMessage_button4_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ContactList_listBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //выбор контакта в списке
        {
            ListBox listBox = (ListBox)sender;
            ContactListIndex = listBox.SelectedIndex;
            chatBody_listBox.ItemsSource = contacts[ContactListIndex].MessageHistory; //обновление списка сообщений в форме
            chatName_textbox.Text = contacts[ContactListIndex].UserName; //обновление имени контакта над списком сообщений
        }

        private void sendMessage_button_Click(object sender, RoutedEventArgs e)
        {
            if (ContactListIndex != -1) //проверка, что выбран контакт
            {
                contacts[ContactListIndex].MessageHistory.Add(new Message(sendMessage_textBox.Text, DateTime.Now.ToString("t"), true)); //добавляем сообщение в историю, нужен sql запрос
                chatBody_listBox.ItemsSource = null; //обнуляем listbox, иначе не работает следующая строка
                chatBody_listBox.ItemsSource = contacts[ContactListIndex].MessageHistory; //обновление списка сообщений в форме
                chatBody_scroll.ScrollToEnd();
                sendMessage_textBox.Text = "Input message";
            }
        }

        private void sendMessage_textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sendMessage_textBox.Text == "Input message")
            {
                sendMessage_textBox.Text = "";
            }
        }

        private void sendMessage_textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sendMessage_textBox.Text))
            {
                sendMessage_textBox.Text = "Input message";
            }
        }

        private void buttonChat_Click(object sender, RoutedEventArgs e)
        {
        }

        private void buttonGroup_Click(object sender, RoutedEventArgs e)
        {
        }

        private void buttonSettings_Click(object sender, RoutedEventArgs e)
        {
        }

        private void buttonSupport_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DockPanel_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ContactListIndex != -1) //проверка, что выбран контакт
                {
                    if (sendMessage_textBox.Text != "")
                    {
                        contacts[ContactListIndex].MessageHistory.Add(new Message(sendMessage_textBox.Text, DateTime.Now.ToString("t"), true)); //добавляем сообщение в историю, нужен sql запрос
                        chatBody_listBox.ItemsSource = null; //обнуляем listbox, иначе не работает следующая строка
                        chatBody_listBox.ItemsSource = contacts[ContactListIndex].MessageHistory; //обновление списка сообщений в форме
                        chatBody_scroll.ScrollToEnd();
                        sendMessage_textBox.Text = "";
                    }
                }
            }
        }

        private void DockPanel_KeyUp_Searching(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (search_textbox.Text != "")
                {
                    string query = $"SELECT * FROM Account WHERE login = '{search_textbox.Text}';";
                    DBconnection dBconnection = new DBconnection();
                    dBconnection.ConnectDB();
                    var result = dBconnection.SelectQuery(query);
                    if (result.Read())
                    {
                        foreach (var contact in contacts)
                        {
                            if (result.GetString(1) == contact.UserName)
                            {
                                return;
                            }
                        }
                        Message message = new Message("Start", "start of correspondence", true);
                        List<Message> messages = new List<Message>();
                        messages.Add(message);
                        contacts.Add(new Contact(messages, result.GetString(1), result.GetString(4)));

                    }
                }
            }
        }
    }
}
