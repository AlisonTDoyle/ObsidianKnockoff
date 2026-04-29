using System;
using System.Collections.Generic;
using System.Globalization;
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
        public Note(string content)
        {
            Content = content;
        }

        public Note(string title, string content)
        {
            Title = title; 
            Content = content;
        }

        // methods
        public override string ToString()
        {
            return Title;
        }

        public string GetFileName()
        {
            string fileName = Title.Replace(" ", "_").ToLower();
            fileName = $"{fileName}.txt";

            return fileName;
        }

        public void SetNoteTitleFromFileName(string fileName)
        {
            string title = fileName.Replace("_", "").Replace(".txt", "");
            title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title);

            Title = title;
        }
    }
}
