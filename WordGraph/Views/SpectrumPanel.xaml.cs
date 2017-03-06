using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using NewsSpectrum.Models;

namespace NewsSpectrum.Views
{
    /// <summary>
    /// Interaction logic for SpectrumPanel.xaml
    /// </summary>
    public partial class SpectrumPanel : UserControl
    {
        public SpectrumPanel()
        {
            InitializeComponent();
        }

        private static double MaxDis = 200;
        private static double MinDis = 110;
        private static double RandomDis = 110;
        private static Random Rand = new Random(DateTime.UtcNow.Millisecond);
        private static Point Center = new Point(600, 200);

        public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register(
            "ItemSource", typeof(IEnumerable<ResultModel>), typeof(SpectrumPanel), new PropertyMetadata(null));

        public IEnumerable<ResultModel> ItemSource
        {
            get { return (IEnumerable<ResultModel>)GetValue(ItemSourceProperty); }
            set
            {
                SetValue(ItemSourceProperty, value);
            }
        }

        public static readonly DependencyProperty SearchContentProperty = DependencyProperty.RegisterAttached(
            "SearchContent", typeof(string), typeof(SpectrumPanel), new PropertyMetadata(null, SearchContent_PropertyChanged));

        public string SearchContent
        {
            get { return (string)GetValue(SearchContentProperty); }
            set { SetValue(SearchContentProperty, value); }
        }

        private static void SearchContent_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as SpectrumPanel;
            if (me == null) return;
        }

        public static readonly DependencyProperty ReloadSignalProperty = DependencyProperty.RegisterAttached(
            "ReloadSignal", typeof(object), typeof(SpectrumPanel), new PropertyMetadata(null, ReloadSignal_PropertyChanged));

        public object ReloadSignal
        {
            get { return (object)GetValue(ReloadSignalProperty); }
            set { SetValue(ReloadSignalProperty, value); }
        }

        private static void ReloadSignal_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as SpectrumPanel;
            if (me == null) return;
            if (me.ItemSource == null || me.ItemSource.Count() < 10) return;
            var data = me.ItemSource.ToList();
            double maxDis = data.Max(r => r.Distance);
            double minDis = data.Min(r => r.Distance);
            double step = maxDis - minDis;
            if (step < 0.001)
                step = 0.001;
            int k = 0;
            me.canvas.Children.Clear();
            var s = new CircleControl
            {
                RadixValue = "100",
                Fill = Brushes.Orange,
                Text = me.SearchContent
            };
            s.SetValue(Canvas.LeftProperty, Center.X - 50);
            s.SetValue(Canvas.TopProperty, Center.Y - 50);

            RandomDis = Rand.Next(-10, 10) + MinDis;
            foreach (var record in data)
            {
                var circle = new CircleControl
                {
                    RadixValue = "80",
                    Fill = Brushes.DodgerBlue,
                    Text = record.Content
                };
                DealPosition(circle, k++, (record.Distance - minDis) / step, record);
                var line = new Line
                {
                    X1 = record.MarginLeft,
                    Y1 = record.MarginTop,
                    X2 = Center.X,
                    Y2 = Center.Y,
                    Stroke = circle.Fill,
                    StrokeThickness = 3
                };
                me.canvas.Children.Add(line);
                me.canvas.Children.Add(circle);
            }
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = i + 1; j < data.Count; j++)
                {
                    if (data[i].ResultCollection.ResultList.FirstOrDefault(n => n.Content.Equals(data[j].Content)) !=
                        null)
                    {
                        var line = new Line
                        {
                            X1 = data[i].MarginLeft,
                            Y1 = data[i].MarginTop,
                            X2 = data[j].MarginLeft,
                            Y2 = data[j].MarginTop,
                            Stroke = Brushes.PaleGreen,
                            StrokeThickness = 2
                        };
                        me.canvas.Children.Insert(0, line);
                    }
                }
            }

            me.canvas.Children.Add(s);
        }

        private static void DealPosition(CircleControl circle, int index, double distance, ResultModel result)
        {
            if (index % 2 == 1)
            {
                index = 10 - index;
            }
            double val = (MaxDis - distance * (MaxDis - RandomDis));
            double left = Math.Cos(1.0 * index / 10 * 2 * Math.PI) * val + Center.X;
            double top = Math.Sin(1.0 * index / 10 * 2 * Math.PI) * val + Center.Y;
            result.MarginTop = top;
            result.MarginLeft = left;
            circle.SetValue(Canvas.TopProperty, result.MarginTop - circle.rectangle.RadiusX / 2);
            circle.SetValue(Canvas.LeftProperty, result.MarginLeft - circle.rectangle.RadiusX / 2);
        }
    }
}
