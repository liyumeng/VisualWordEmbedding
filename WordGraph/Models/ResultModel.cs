using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsSpectrum.Utils;

namespace NewsSpectrum.Models
{
    public class ResultModel : NotificationObject, IComparable<ResultModel>
    {
        public ResultModel()
        {
            ResultCollection = new ResultCollection();
        }

        public ResultModel(string content, double distance)
        {
            m_content = content;
            m_distance = distance;
            ResultCollection = new ResultCollection();
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

        private double m_distance;

        public double Distance
        {
            get { return m_distance; }
            set
            {
                if (value != m_distance)
                {
                    m_distance = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Distance"));
                }
            }
        }

        private string m_index;

        public string Index
        {
            get { return m_index; }
            set
            {
                if (value != m_index)
                {
                    m_index = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Index"));
                }
            }
        }

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

        public int CompareTo(ResultModel other)
        {
            if (Distance > other.Distance)
                return -1;
            if (Distance < other.Distance)
                return 1;
            return 0;
        }

        private string m_type;

        public string Type
        {
            get { return m_type; }
            set
            {
                if (value != m_type)
                {
                    m_type = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Type"));
                }
            }
        }

        public double MarginLeft { get; set; }
        public double MarginTop { get; set; }
    }
}
