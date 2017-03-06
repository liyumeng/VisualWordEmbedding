using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WordGraph.Views
{
    /// <summary>
    /// Interaction logic for CircleControl.xaml
    /// </summary>
    public partial class CircleControl : UserControl
    {
        public CircleControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text", typeof(string), typeof(CircleControl), new PropertyMetadata(null, Text_PropertyChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void Text_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as CircleControl;
            if (me == null) return;
            me.textBox.Text = e.NewValue as string;
        }

        public static readonly DependencyProperty FillProperty = DependencyProperty.RegisterAttached(
            "Fill", typeof(Brush), typeof(CircleControl), new PropertyMetadata(null, Fill_PropertyChanged));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        private static void Fill_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as CircleControl;
            if (me == null) return;
            me.rectangle.Fill = e.NewValue as Brush;
        }

        public static readonly DependencyProperty RadixValueProperty = DependencyProperty.RegisterAttached(
            "RadixValue", typeof(string), typeof(CircleControl), new PropertyMetadata(null, RadixValue_PropertyChanged));

        public string RadixValue
        {
            get { return (string)GetValue(RadixValueProperty); }
            set { SetValue(RadixValueProperty, value); }
        }

        private static void RadixValue_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as CircleControl;
            if (me == null) return;
            double val = 80;
            if (double.TryParse(e.NewValue.ToString(), out val))
            {
                me.rectangle.Height = val;
                me.rectangle.Width = val;
                me.rectangle.RadiusX = val;
                me.rectangle.RadiusY = val;
            }

        }
    }
}
