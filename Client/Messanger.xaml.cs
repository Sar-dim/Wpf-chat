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
using System.Threading;
using System.Net.Sockets;
using DBConnection;
using System.Runtime.CompilerServices;
using System.Diagnostics.Eventing.Reader;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Messanger : Window
    {
        
        public Net net;
        public List<Contact> contacts;
        
        public int ContactListIndex = -1;
        public Messanger()
        { }
        public Messanger(string login)
        {
            InitializeComponent();
            contacts = new List<Contact>();
            DBconnection dBconnection = new DBconnection();
            dBconnection.ConnectDB();
            //Заполнение данных MyContact
            string query = $"SELECT DISTINCT * FROM Account WHERE login = '{login}'";
            var result = dBconnection.SelectQuery(query);
            if (result.Read())
            {
                string name = result.GetString(1);
                string status = result.GetString(4);
                MyContact.userName_textBlock.Text = name;
                MyContact.userTitle_textBlock.Text = status;
            }
            dBconnection.ForceClose();
            //Заполнение ContactList
            List<string> contactsString = new List<string>();
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
            dBconnection.ForceClose();
            //Добавление контактам сообщений
            List<Message> tempMessages = new List<Message>();        
            foreach (var contactString in contactsString)
            {
                dBconnection.ConnectDB();
                query = $"SELECT * FROM Messages WHERE (sender = '{contactString}' AND recepient = '{login}') OR (sender = '{login}' AND recepient = '{contactString}');";
                result = dBconnection.SelectQuery(query);
                while (result.Read())
                {
                    if (result.GetString(1) == contactString)
                    {
                        tempMessages.Add(new Message(result.GetString(3), result.GetString(5), false));
                    }
                    else if (result.GetString(2) == contactString)
                    {
                        tempMessages.Add(new Message(result.GetString(3), result.GetString(5), true));
                    }
                }
                contacts.Add(new Contact(new List<Message>(tempMessages), contactString, "")); //создание контакта с сообщениями
                tempMessages.Clear();
                dBconnection.ForceClose();
            }
            ContactList_listBox.ItemsSource = contacts; //обновление списка контактов на форме
            //Подключение к серверу и передача ему login
            net = new Net(login);
            
            Label_ServerConnect.Foreground = Brushes.Green;
            Label_ServerConnect.Content = "Сервер подключен";
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
            chatBody_scroll.ScrollToEnd();
        }
        private void sendMessage_button_Click(object sender, RoutedEventArgs e)
        {
            if (ContactListIndex != -1) //проверка, что выбран контакт
            {
                AddNewMessage(sender, e);
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
                        AddNewMessage(sender, e);
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
                        List<Message> messages = new List<Message>();
                        messages.Add(new Message("Start", DateTime.Now.ToString("t"), true));
                        contacts.Add(new Contact(new List<Message>(messages), result.GetString(1), result.GetString(4)));
                        ContactList_listBox.ItemsSource = null;
                        ContactList_listBox.ItemsSource = contacts; //обновление списка контактов на форме
                        search_textbox.Text = "Searching";
                    }
                }
            }
        }
        private void Close_button_Click(object sender, RoutedEventArgs e)
        {
            net.Disconnect();
            this.Close();
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        //Добавление нового сообщения
        private void AddNewMessage(object sender, RoutedEventArgs e)
        {
            contacts[ContactListIndex].MessageHistory.Add(new Message(sendMessage_textBox.Text, DateTime.Now.ToString("t"), true));
            chatBody_listBox.ItemsSource = null; //обнуляем listbox, иначе не работает следующая строка
            chatBody_listBox.ItemsSource = contacts[ContactListIndex].MessageHistory; //обновление списка сообщений в форме
            chatBody_scroll.ScrollToEnd();
            net.SendMessage($"NewMessage{MyContact.userName_textBlock.Text}&{contacts[ContactListIndex].userName_textBlock.Text}&{sendMessage_textBox.Text}&{DateTime.Now.ToString("d")}&{DateTime.Now.ToString("t")}");
        }
        //Добавление нового полученного сообщения
        public void AddNewReceivedMessage(string textMessage, string sender, string date, string time)
        {
            bool foundContact = false;
            foreach (var contact in contacts)
            {
                if (contact.userName_textBlock.Text == sender) //Если отправитель есть в контактах
                {
                    contact.MessageHistory.Add(new Message(textMessage, time, false));//добавляем ему полученное соощение
                    foundContact = true;
                    if (contact == contacts[ContactListIndex]) //Если этот контакт выбран сейчас, то отображаем сообщение
                    {
                        chatBody_listBox.ItemsSource = null; //обнуляем listbox, иначе не работает следующая строка
                        chatBody_listBox.ItemsSource = contacts[ContactListIndex].MessageHistory; //обновление списка сообщений в форме
                        chatBody_scroll.ScrollToEnd();
                    }
                }
            }
            if (!foundContact) //Если отправителя нет в контактах
            {
                List<Message> newMessage = new List<Message>();
                newMessage.Add(new Message(textMessage, time, false));
                contacts.Add(new Contact(new List<Message>(newMessage), sender, ""));
            }          
        }     
    }
}
