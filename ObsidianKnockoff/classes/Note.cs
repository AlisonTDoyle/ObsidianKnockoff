using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObsidianKnockoff.Classes
{
    internal class Note
    {
        // properties
        public string Title { get; set; }
        public string Content { get; set; }

        // constructor
        public Note() { }

        public Note(string title, string content)
        {
            Title = title; 
            Content = content;
        }

        // methods
        public override ToString()
        {
            return Title;
        }
    }
}
