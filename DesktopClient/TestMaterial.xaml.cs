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
    /// Interaction logic for TestMaterial.xaml
    /// </summary>
    public partial class TestMaterial : Window
    {
        public TestMaterial()
        {
            InitializeComponent();
        }

        private void btnWork(object sender, RoutedEventArgs e)
        {
            var bc = new BrushConverter();

            // Create Card
            MaterialDesignThemes.Wpf.Card newcourse = new MaterialDesignThemes.Wpf.Card
            {
                Width = 280,
                Height = 129,
                Background = (Brush)bc.ConvertFrom("#f0f2f5"),
                Margin = new Thickness(0, 0, 29, 29)
            };


            // Create Grid (is inside card)
            Grid ncgrid = new Grid();
            ncgrid.Width = 280;
            ncgrid.Height = 129;

            // Create and Add Grid Column
            ColumnDefinition gridColumn0 = new ColumnDefinition();
            ncgrid.ColumnDefinitions.Add(gridColumn0);

            // Create and Add Grid Rows
            RowDefinition gridRow0 = new RowDefinition
            {
                Height = new GridLength(94)
            };
            RowDefinition gridRow1 = new RowDefinition
            {
                Height = new GridLength(35)
            };
            ncgrid.RowDefinitions.Add(gridRow0);
            ncgrid.RowDefinitions.Add(gridRow1);


            // Create Course Number Text
            TextBlock coursenum = new TextBlock
            {
                Text = "CS 310",
                Foreground = (Brush)bc.ConvertFrom("#004D40"),
                FontWeight = FontWeights.SemiBold,
                FontSize = 22,
                Margin = new Thickness(11, 17, 0, 49)
            };
            Grid.SetRow(coursenum, 0);  // Add coursenum to grid row 0
            Grid.SetColumn(coursenum, 0);

            newcourse.Content = ncgrid;

            // Add new course card to WrapPanel
            ClassPanel.Children.Add(newcourse);
        }
    }
}
