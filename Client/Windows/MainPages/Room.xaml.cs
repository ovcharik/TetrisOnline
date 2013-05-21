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

namespace Client.Windows.MainPages
{
    public partial class Room : Page
    {
        public Room()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Models.Room r = this.DataContext as Models.Room;
            if (r == ServerSide.Connection.Instance.Data.CurrentRoom)
            {
                String data = this.TextBoxMessage.Text;
                if (data.Length == 0) return;

                ServerSide.Sender.SendRoomMsg(data);

                r.AddMessage(new Models.Message
                {
                    Type = Models.Message.MessageType.Output,
                    DateTime = DateTime.Now,
                    Data = data,
                    User = ServerSide.Connection.Instance.Data.CurentUser
                });

                r.CurrentMessage = "";
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Models.User user = ServerSide.Connection.Instance.Data.CurentUser;
            if (user != null)
            {
                if (user.isMember) ServerSide.Sender.LeaveRoom();
                if (user.isWatcher) ServerSide.Sender.NotWatchRoom();
                ServerSide.NavigationService.GotoMainPage();
            }
        }

        private void ListView_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            ListView l = sender as ListView;
            Models.User u = l.SelectedItem as Models.User;
            if (u != null)
            {
                Windows.Messages.Instance.AddUser(u, true);
            }
        }

        private void ScrollViewer_ScrollChanged_1(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            if (sv == null) return;
            if (sv.VerticalOffset == sv.ScrollableHeight)
            {
                sv.ScrollToBottom();
            }
        }
    }
}
