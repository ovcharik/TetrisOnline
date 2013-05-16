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
        public MainPage(MainWindow owner)
        {
            this.Owner = owner;
            InitializeComponent();
        }

        public void OnRaiseUpdateId(object sender, String json)
        {
        }

        public void OnRaiseUpdateUserList(object sender, String json)
        {
            List<JsonBaseObject> userList = JsonConvert.DeserializeObject<List<JsonBaseObject>>(json);

            this.Dispatcher.Invoke(delegate
            {
                this.listBoxUsers.Items.Clear();
                foreach (var u in userList)
                {
                    this.listBoxUsers.Items.Add(u);
                }
            });
        }

        public void OnRaiseSignedIn(object sender, String json)
        {
            JsonBaseObject user = JsonConvert.DeserializeObject<JsonBaseObject>(json);
            this.Dispatcher.Invoke(delegate
            {
                this.listBoxUsers.Items.Add(user);
            });
        }

        public void OnRaiseSignedOut(object sender, String json)
        {
            JsonBaseObject user = JsonConvert.DeserializeObject<JsonBaseObject>(json);
            this.Dispatcher.Invoke(delegate
            {
                foreach (JsonBaseObject u in this.listBoxUsers.Items)
                {
                    if (u.Int == user.Int)
                    {
                        user = u;
                        break;
                    }
                }
                this.listBoxUsers.Items.Remove(user);
            });
        }
    }
}
