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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DesktopClient.Data;
using DesktopClient.Models;

namespace DesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin(object sender, RoutedEventArgs e)
        {
            using UserDbContext db = new UserDbContext();
            var _user = db.tblUsers.FirstOrDefault(a => a.Email == tbEmail.Text);
            if (_user != null && tbPass.Text == _user.Password)
            {
                /*
                loginColor.Fill = new SolidColorBrush(Colors.Green);
                loginText.Text = "Logged In";
                userinfo.Text = _user.Firstname + " " + _user.Lastname;
                */

                Home _home = new Home(_user);
                _home.Show();
                this.Close();
            }
            else
            {
                loginText.Text = "Invalid Email or Password";
                loginColor.Fill = new SolidColorBrush(Colors.Red);
                userinfo.Text = "";
            }
        }


    }
}
