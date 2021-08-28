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
using MaterialDesignThemes;
using MaterialDesignColors;

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


            // Create Course Name Text
            TextBlock coursename = new TextBlock
            {
                Text = "Data Structures and Algorithms II",
                Foreground = (Brush)bc.ConvertFrom("#004D40"),
                FontWeight = FontWeights.Light,
                FontSize = 12,
                Margin = new Thickness(12, 45, 0, 22)
            };
            Grid.SetRow(coursename, 0);  // Add coursename to grid row 0


            // Create rectangle for row 1 green background color
            Rectangle rect = new Rectangle
            {
                Fill = (Brush)bc.ConvertFrom("#1b807c")
            };
            Grid.SetRow(rect, 1);


            // Create # of Transcripts Text
            TextBlock transcripts = new TextBlock
            {
                Text = "7 Transcripts",
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 10,
                Width = 94,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(13, 0, 0, 0)
            };
            Grid.SetRow(transcripts, 1);  // Add transcripts to grid row 1

            ncgrid.Children.Add(coursenum);
            ncgrid.Children.Add(coursename);
            ncgrid.Children.Add(rect);
            ncgrid.Children.Add(transcripts);

            ncgrid.MouseLeftButtonDown += (sen, evg) =>
            {
                CoursePage win = new CoursePage();
                win.Show();
            };


            newcourse.Content = ncgrid;

            // Add new course card to WrapPanel
            ClassPanel.Children.Add(newcourse);
        }
    }
}
