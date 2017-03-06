using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WordGraph.Models;
using WordGraph.Utils;
using System.IO;
using System.Threading;

namespace WordGraph.ViewModels
{
    public class SearchNewsViewModel : NotificationObject
    {
        public SearchNewsViewModel()
        {
            SearchCommand = new ActionCommand();
            SearchCommand.Executing += SearchCommand_Executing;
            SearchNewsCommand = new ActionCommand();
            SearchNewsCommand.Executing += SearchNewsCommand_Executing;

            ListBoxDoubleClickCommand = new ActionCommand();
            ListBoxDoubleClickCommand.Executing += ListBoxDoubleClickCommand_Executing;
            NewsListBoxDoubleClickCommand = new ActionCommand();
            NewsListBoxDoubleClickCommand.Executing += NewsListBoxDoubleClickCommand_Executing;
            ResultCollection = new ResultCollection();
            NewsResultCollection = new ResultCollection();
            StatusInfo = "Ready";
            StatusColor = 1;
            IsLoading = true;
            CToken = new CancellationTokenSource();
            LoadDictAsync();
        }

        public CancellationTokenSource CToken { get; set; }

        private ResultCollection m_resultCollection;

        public ResultCollection ResultCollection
        {
            get { return m_resultCollection; }
            set
            {
                if (value != m_resultCollection)
                {
                    m_resultCollection = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ResultCollection"));
                }
            }
        }

        private string m_statusInfo;

        public string StatusInfo
        {
            get { return m_statusInfo; }
            set
            {
                if (value != m_statusInfo)
                {
                    m_statusInfo = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("StatusInfo"));
                }
            }
        }

        public ActionCommand SearchCommand { get; set; }

        public ActionCommand ListBoxDoubleClickCommand { get; set; }

        private void ListBoxDoubleClickCommand_Executing(object sender, ExecutingEventArgs e)
        {
            var record = e.Parameter as ResultModel;
            if (record == null) return;
            ResultCollection.SearchContent = record.Content;
            SearchCommand.Execute(null);
        }

        private void SearchCommand_Executing(object sender, ExecutingEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(ResultCollection.SearchContent) || IsLoading)
                return;
            StatusInfo = String.Format("Searching '{0}' ...", ResultCollection.SearchContent);
            StatusColor = 1;
            if (IsSearching)
            {
                CToken.Cancel();
                CToken = new CancellationTokenSource();
            }
            IsSearching = true;
            var data = VectorFinder.FindWord(ResultCollection.SearchContent, CToken.Token);
            ResultCollection.ResultList.Clear();
            if (data == null) return;
            int i = 0;
            foreach (var record in data)
            {
                ResultCollection.ResultList.Add(record);
                record.Index = String.Format("{0}.", ++i);
            }
            IsBeginSearching = true;
            ReloadSignal = true;
            FindRelations(CToken.Token);

        }

        public void FindRelations(CancellationToken token)
        {
            var task = new Task(() =>
            {
                if (token.IsCancellationRequested)
                    return;
                for (int i = 0; i < ResultCollection.ResultList.Count; i++)
                {
                    ResultModel record = ResultCollection.ResultList[i];
                    StatusInfo = String.Format("Searching '{0}' ...", record.Content);
                    var data = VectorFinder.FindWord(record.Content, token);
                    record.ResultCollection.ResultList.Clear();
                    if (data == null) return;
                    foreach (var r in data)
                    {
                        record.ResultCollection.ResultList.Add(r);
                    }
                    if (token.IsCancellationRequested)
                        return;
                }
                ReloadSignal = true;
                StatusInfo = "Ready";
                StatusColor = 0;
                IsSearching = false;
            });
            task.Start();
        }

        private void LoadDictAsync()
        {
            var task = new Task(() =>
            {
                IsLoading = true;

                try
                {
                    StatusInfo = "Loading dictionary...";
                    VectorFinder.LoadDict();
                    // StatusInfo = "Loading page vectors...";
                    // VectorFinder.LoadPageVectors();
                }
                catch (IOException e)
                {
                    StatusInfo = e.Message;
                    StatusColor = 2;
                    return;
                }


                StatusInfo = "Ready";
                IsLoading = false;
                StatusColor = 0;
                //SearchCommand.Execute(null);

            });
            task.Start();
        }

        private void LoadPageVectorsAsync()
        {
            var task = new Task(() =>
            {
                IsLoading = true;
                StatusInfo = "Loading page vectors...";
                try
                {
                    VectorFinder.LoadPageVectors();
                }
                catch (IOException e)
                {
                    StatusInfo = e.Message;
                    StatusColor = 2;
                    return;
                }

                StatusInfo = "Ready";
                IsLoading = false;
                StatusColor = 0;
                //SearchCommand.Execute(null);

            });
            task.Start();
        }

        private bool m_reloadSignal;

        public bool ReloadSignal
        {
            get { return m_reloadSignal; }
            set
            {
                m_reloadSignal = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ReloadSignal"));
            }
        }

        private bool m_isLoading;

        public bool IsLoading
        {
            get { return m_isLoading; }
            set
            {
                if (value != m_isLoading)
                {
                    m_isLoading = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsLoading"));
                }
            }
        }

        private int m_statusColor;

        public int StatusColor
        {
            get { return m_statusColor; }
            set
            {
                if (value != m_statusColor)
                {
                    m_statusColor = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("StatusColor"));
                }
            }
        }

        private bool m_isBeginSearching;

        public bool IsBeginSearching
        {
            get { return m_isBeginSearching; }
            set
            {
                if (value != m_isBeginSearching)
                {
                    m_isBeginSearching = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsBeginSearching"));
                }
            }
        }

        private bool m_isSearching;

        public bool IsSearching
        {
            get { return m_isSearching; }
            set
            {
                if (value != m_isSearching)
                {
                    m_isSearching = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsSearching"));
                }
            }
        }

        private bool m_isShowNewsContent;

        public bool IsShowNewsContent
        {
            get { return m_isShowNewsContent; }
            set
            {
                if (value != m_isShowNewsContent)
                {
                    m_isShowNewsContent = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsShowNewsContent"));
                }
            }
        }

        private ResultCollection m_newsResultCollection;

        public ResultCollection NewsResultCollection
        {
            get { return m_newsResultCollection; }
            set
            {
                if (value != m_newsResultCollection)
                {
                    m_newsResultCollection = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("NewsResultCollection"));
                }
            }
        }

        public ActionCommand SearchNewsCommand { get; set; }

        private void SearchNewsCommand_Executing(object sender, ExecutingEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(NewsResultCollection.SearchContent) || IsLoading)
                return;
            StatusInfo = String.Format("Searching '{0}' ...", NewsResultCollection.SearchContent);
            StatusColor = 1;
            IsShowNewsContent = false;
            if (IsSearching)
            {
                CToken.Cancel();
                CToken = new CancellationTokenSource();
            }

            IsSearching = true;
            var data = VectorFinder.FindPage(NewsResultCollection.SearchContent, CToken.Token);
            NewsResultCollection.ResultList.Clear();
            if (data == null) return;
            int i = 0;
            foreach (var record in data)
            {
                NewsResultCollection.ResultList.Add(record);
                record.Index = String.Format("{0}.", ++i);
            }
            IsBeginSearching = true;
            ReloadNewsSignal = true;
            FindNewsRelations(CToken.Token);
            //IsSearching = false;
            //StatusInfo = "Ready";
            //StatusColor = 0;
        }

        private bool m_reloadNewsSignal;

        public bool ReloadNewsSignal
        {
            get { return m_reloadNewsSignal; }
            set
            {
                m_reloadNewsSignal = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ReloadNewsSignal"));
            }
        }

        private string m_newsContent;

        public string NewsContent
        {
            get { return m_newsContent; }
            set
            {
                if (value != m_newsContent)
                {
                    m_newsContent = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("NewsContent"));
                }
            }
        }
        public ActionCommand NewsListBoxDoubleClickCommand { get; set; }

        private void NewsListBoxDoubleClickCommand_Executing(object sender, ExecutingEventArgs e)
        {
            var record = e.Parameter as ResultModel;
            if (record == null) return;

            IsShowNewsContent = true;
            try
            {
                NewsContent = PageVector.ReadFile(record.Content);
            }
            catch (IOException ex)
            {
                StatusInfo = ex.Message;
                StatusColor = 2;
                return;
            }
            //NewsResultCollection.SearchContent = record.Content;
            //SearchCommand.Execute(null);
        }

        public void FindNewsRelations(CancellationToken token)
        {
            var task = new Task(() =>
            {
                if (token.IsCancellationRequested)
                    return;
                for (int i = 0; i < NewsResultCollection.ResultList.Count; i++)
                {
                    ResultModel record = NewsResultCollection.ResultList[i];
                    StatusInfo = String.Format("Searching '{0}' ...", record.Content);
                    var data = VectorFinder.Find(record.Content, token);
                    record.ResultCollection.ResultList.Clear();
                    if (data == null) return;
                    foreach (var r in data)
                    {
                        record.ResultCollection.ResultList.Add(r);
                    }
                    if (token.IsCancellationRequested)
                        return;
                }
                ReloadNewsSignal = true;
                StatusInfo = "Ready";
                StatusColor = 0;
                IsSearching = false;
            });
            task.Start();
        }
    }
}
