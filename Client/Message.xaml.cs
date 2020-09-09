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
    /// Логика взаимодействия для Message.xaml
    /// </summary>
    public partial class Message : UserControl
    {
        public bool SentByMe { get; set; }
        public string MessageTime { get; set; }
        public string MessageText { get; set; }
        public void Orientation(bool sentByMe)
        {
            if (sentByMe)
            {
                message_grid.HorizontalAlignment = HorizontalAlignment.Left;
                bubble.HorizontalAlignment = HorizontalAlignment.Left;
                messageTime_textBlock.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else
            {
                message_grid.HorizontalAlignment = HorizontalAlignment.Right;
                bubble.HorizontalAlignment = HorizontalAlignment.Right;
                messageTime_textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }
        public Message()
        {
            InitializeComponent();
        }
        public Message(string messageText, string messageTime, bool sentByMe)
        {
            this.MessageText = messageText;
            this.MessageTime = messageTime;

            InitializeComponent();
            Orientation(sentByMe);
            messageText_textblock.Text = MessageText;
            messageTime_textBlock.Text = MessageTime;
        }
    }
}
