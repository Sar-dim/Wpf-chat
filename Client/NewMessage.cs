using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    class NewMessage
    {
        public string Sender { get; set; }
        public string Recepient { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public bool Is_Deleted { get; set; }
        public NewMessage(string sender, string recepient, string text, string date, string time)
        {
            Sender = sender;
            Recepient = recepient;
            Text = text;
            Date = date;
            Time = time;
        }
    }
}
