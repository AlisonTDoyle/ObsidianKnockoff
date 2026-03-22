using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObsidianKnockoff.Classes
{
    internal class Message
    {
        // properties
        private string _sender;
        private string _content;
        private DateTime _date;

        public string Sender { get { return _sender; } }
        public string Content { get { return _content; } }
        public DateTime Date { get { return _date; } }

        // constructors
        public Message(string sender, string content)
        {
            _sender = sender;
            _content = content;
            _date = DateTime.Now;
        }

        // methods
    }
}
