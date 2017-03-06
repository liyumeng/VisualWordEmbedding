using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WordGraph.Utils;

namespace WordGraph.Models
{
    public class ResultCollection : NotificationObject
    {
        public ResultCollection()
        {
            ResultList = new ObservableCollection<ResultModel>();
        }

        private string m_searchContent;

        public string SearchContent
        {
            get { return m_searchContent; }
            set
            {
                if (value != m_searchContent)
                {
                    m_searchContent = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("SearchContent"));
                }
            }
        }

        private ObservableCollection<ResultModel> m_resultList;

        public ObservableCollection<ResultModel> ResultList
        {
            get { return m_resultList; }
            set
            {
                if (value != m_resultList)
                {
                    m_resultList = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ResultList"));
                }
            }
        }
    }
}
