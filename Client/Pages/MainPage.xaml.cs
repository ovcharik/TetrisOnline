using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Client.Pages
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainWindow Owner { get; set; }
        
        public MainPage(MainWindow owner)
        {
            InitializeComponent();
            UserList.DataContext = Connection.Instance.Data.Users;
            Owner = owner;
        }

        private void ListView_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            ListView l = sender as ListView;
            Models.User u = l.SelectedItem as Models.User;
            if (u != null)
            {
                MessageWindow.Instance.AddUser(u, true);
            }
        }
    }
}
