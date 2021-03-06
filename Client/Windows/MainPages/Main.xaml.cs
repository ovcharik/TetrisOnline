﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;

namespace Client.Windows.MainPages
{
    public partial class Main : Page
    {
        public Main()
        {
            InitializeComponent();
            UserList.DataContext = ServerSide.Connection.Instance.Data.Users;
            RoomList.DataContext = ServerSide.Connection.Instance.Data.Rooms;
        }

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

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            String name = this.textBoxRoomName.Text;
            Int32 capacity = (Int32)this.comboBoxCapacity.SelectedItem;

            Button b = sender as Button;
            if (b == null) return;
            b.IsEnabled = false;

            new Thread(delegate()
            {
                try
                {
                    ServerSide.Sender.CreateRoom(name, capacity);
                }
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke(delegate
                    {
                        MessageBox.Show(ex.Message);
                        b.IsEnabled = true;
                    });
                }
            }).Start();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ServerSide.Connection.Instance.Disconnect();
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b == null) return;
            Models.Room r = b.DataContext as Models.Room;
            if (r == null) return;
            ServerSide.Sender.EnterRoom(r.Id);
        }

        private void ButtonWatch_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b == null) return;
            Models.Room r = b.DataContext as Models.Room;
            if (r == null) return;
            ServerSide.Sender.WatchRoom(r.Id);
        }
    }
}
