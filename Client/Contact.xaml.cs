using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Contact.xaml
    /// </summary>
    public partial class Contact : UserControl
    {
        public Contact()
        {
            InitializeComponent();
        }
        public Contact(List<Message> messageHistory, string userName, string userTitle)
        {
            InitializeComponent();
            this.MessageHistory = messageHistory;
            this.UserName = userName;
            this.UserTitle = userTitle;

        }
        public List<Message> MessageHistory { get; set; }
        public ImageSource userImage
        {
            get { return userPic.Source; }
            set { userPic.Source = value; }

        }
        public string UserName
        {
            get { return userName_textBlock.Text; }
            set { userName_textBlock.Text = value; }
        }
        public string UserTitle
        {
            get { return userTitle_textBlock.Text; }
            set { userTitle_textBlock.Text = value; }
        }
    }
}
