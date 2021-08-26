using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DesktopClient
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();
        }

        private void btnAdd(object sender, RoutedEventArgs e)
        {
            /*
            var newbutton = new Button() { Content = "myButton" }; // Creating button
            ClassPanel.Children.Add(newbutton);
            */
            AddCourse();

        }

        private void AddCourse()
        {
            var newcourse = new MaterialDesignThemes.Wpf.Card();
            newcourse.Width = 280;
            var bc = new BrushConverter();
            newcourse.Background = (Brush)bc.ConvertFrom("#f0f2f5");
            newcourse.Margin = new Thickness(0, 0, 29, 29);
            ClassPanel.Children.Add(newcourse);
        }
    }
}
