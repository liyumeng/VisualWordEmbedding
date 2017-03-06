using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordGraph.Utils;

namespace WordGraph.Models
{
    public class NewsModel : NotificationObject
    {
        private string m_title;

        public string Title
        {
            get { return m_title; }
            set
            {
                if (value != m_title)
                {
                    m_title = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Title"));
                }
            }
        }

        private string m_content;

        public string Content
        {
            get { return m_content; }
            set
            {
                if (value != m_content)
                {
                    m_content = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Content"));
                }
            }
        }
    }
}
