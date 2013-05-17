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

using Newtonsoft.Json;
using Interface.Json;

namespace Client.Pages
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainWindow Owner { get; set; }
        public ObservableCollection<Models.User> UserCollection { get; set; }
        
        public MainPage(MainWindow owner)
        {
            InitializeComponent();
            DataContext = this;
            Owner = owner;
            UserCollection = new ObservableCollection<Models.User>();
        }

        public void OnRaiseUpdateId(object sender, String json)
        {
        }

        public void OnRaiseUpdateUserList(object sender, String json)
        {
            List<JsonBaseObject> userList = JsonConvert.DeserializeObject<List<JsonBaseObject>>(json);
            this.Dispatcher.Invoke(delegate
            {
                this.UserCollection.Clear();
                foreach (var u in userList)
                {
                    this.UserCollection.Add(new Models.User
                    {
                        Id = u.Int,
                        Name = u.String
                    });
                }
            });
        }

        public void OnRaiseSignedIn(object sender, String json)
        {
            JsonBaseObject user = JsonConvert.DeserializeObject<JsonBaseObject>(json);
            this.Dispatcher.Invoke(delegate
            {
                this.UserCollection.Add(new Models.User
                {
                    Id = user.Int,
                    Name = user.String
                });
            });
        }

        public void OnRaiseSignedOut(object sender, String json)
        {
            JsonBaseObject user = JsonConvert.DeserializeObject<JsonBaseObject>(json);
            this.Dispatcher.Invoke(delegate
            {
                foreach (var i in this.UserCollection)
                {
                    if (i.Id == user.Int)
                    {
                        this.UserCollection.Remove(i);
                        break;
                    }
                }
                
            });
        }

        private void ListView_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            ListView l = sender as ListView;
            Models.User u = l.SelectedItem as Models.User;
            if (u != null)
            {
                MessageWindow msg = new MessageWindow();
                msg.Show();

                Models.MessagesContent mc = new Models.MessagesContent
                {
                    User = u
                };
                msg.TabControlMain.Items.Add(mc);
            }
        }
    }
}
