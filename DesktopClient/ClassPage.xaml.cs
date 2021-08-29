using DesktopClient.Models;
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
    /// Interaction logic for CoursePage.xaml
    /// </summary>
    public partial class ClassPage : Window
    {
        Course _course;
        string _userid;
        public ClassPage(Course course, string userid)
        {
            _course = course;
            _userid = userid;
            InitializeComponent();
            coursenumber.Text = _course.CourseNum;
            // Need to add check so only teachers can start a transcript.

        }

        private void btnTranscribe(object sender, RoutedEventArgs e)
        {

        }
    }
}
