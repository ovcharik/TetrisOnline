using System.Windows.Controls;
using System.Windows.Input;

namespace Client.Windows.MainPages
{
    public partial class Main : Page
    {
        public Main(Windows.Main owner)
        {
            InitializeComponent();
            UserList.DataContext = ServerSide.Connection.Instance.Data.Users;
            Owner = owner;
        }

        // Propetries
        public Windows.Main Owner { get; set; }

        // Form Events Handlers
        private void ListView_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            ListView l = sender as ListView;
            Models.User u = l.SelectedItem as Models.User;
            if (u != null)
            {
                Windows.Messages.Instance.AddUser(u, true);
            }
        }
    }
}
