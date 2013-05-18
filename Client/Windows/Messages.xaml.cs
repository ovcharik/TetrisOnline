using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client.Windows
{
    /// <summary>
    /// Логика взаимодействия для MessageWindow.xaml
    /// </summary>
    public partial class Messages : Window
    {
        private Messages()
        {
            InitializeComponent();
            this.Activated += MessageWindow_Activated;
        }

        // Properties
        private static Messages _Instance = null;
        public static Messages Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new Messages();
                }
                return _Instance;
            }
        }

        // Private Methods
        private void _SendMessage(Models.User user)
        {
            if (user.MsgData.Length > 0)
            {
                try
                {
                    ServerSide.Sender.SendMsg(user.Id, user.MsgData.Clone() as String);

                    Models.Message msg = new Models.Message
                    {
                        User = ServerSide.Connection.Instance.Data.CurentUser,
                        Data = user.MsgData,
                        DateTime = DateTime.Now,
                        Direction = Models.Direction.Output
                    };
                    user.AddMessage(msg);
                    user.MsgData = "";
                    user.ResetNewMsgs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // Public Methods
        public void AddUser(Models.User user, bool force = false)
        {
            this.Show();

            if (!this.TabControlMain.Items.Contains(user))
            {
                this.TabControlMain.Items.Add(user);
            }
            if (force || this.TabControlMain.Items.Count == 1)
                this.TabControlMain.SelectedItem = user;
            if (force)
                this.Activate();
        }

        // Form Events Handlers
        void MessageWindow_Activated(object sender, EventArgs e)
        {
            Models.User u = this.TabControlMain.SelectedItem as Models.User;
            if (u != null) u.ResetNewMsgs();
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            _Instance = null;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b == null) return;

            Models.User u = b.DataContext as Models.User;
            if (u == null) return;

            TabControlMain.Items.Remove(u);
            if (TabControlMain.Items.IsEmpty)
            {
                this.Close();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b == null) return;

            Models.User u = b.DataContext as Models.User;
            if (u == null) return;

            _SendMessage(u);
        }

        private void TextBox_KeyDown_2(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tb = sender as TextBox;
                if (tb == null) return;

                Grid p = tb.Parent as Grid;
                if (p == null) return;

                Models.User u = p.DataContext as Models.User;
                u.MsgData = tb.Text;
                if (u == null) return;

                if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                {
                    this._SendMessage(u);
                }
            }
        }

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (this.IsActive)
            {
                Models.User u = this.TabControlMain.SelectedItem as Models.User;
                if (u != null) u.ResetNewMsgs();
            }
        }

        private void ScrollViewer_ScrollChanged_1(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            if (sv == null) return;

            if (this.IsActive)
            {
                if (sv.VerticalOffset == sv.ScrollableHeight)
                {
                    sv.ScrollToBottom();
                }
            }
        }
    }
}
