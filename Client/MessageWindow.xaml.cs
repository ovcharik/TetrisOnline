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

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        private static MessageWindow _Instance = null;
        public static MessageWindow Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new MessageWindow();
                return _Instance;
            }
        }
        private MessageWindow()
        {
            InitializeComponent();
        }

        public void AddUser(Models.User user)
        {
            this.Show();

            if (!this.TabControlMain.Items.Contains(user))
            {
                this.TabControlMain.Items.Add(user);
            }
            this.TabControlMain.SelectedItem = user;
            this.Activate();
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            _Instance = null;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                Models.User u = b.DataContext as Models.User;
                if (u != null)
                {
                    TabControlMain.Items.Remove(u);
                    if (TabControlMain.Items.IsEmpty)
                    {
                        this.Close();
                    }
                }
            }
        }
    }
}
