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
            this.TabControlMain.SelectionChanged += TabControlMain_SelectionChanged;
            this.Activated += MessageWindow_Activated;
        }

        void MessageWindow_Activated(object sender, EventArgs e)
        {
            Models.User u = this.TabControlMain.SelectedItem as Models.User;
            if (u != null) u.ResetNewMsgs();
        }

        void TabControlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsActive)
            {
                TabControl tc = sender as TabControl;
                Models.User u = tc.SelectedItem as Models.User;
                if (u != null) u.ResetNewMsgs();
            }
        }

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

        private void SendMessage(Models.User user)
        {
            try
            {
                Sender.SendMsg(user.Id, user.MsgData.Clone() as String);

                Models.Message msg = new Models.Message
                {
                    User = Connection.Instance.Data.CurentUser,
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                Models.User u = b.DataContext as Models.User;
                if (u != null && u.MsgData.Length > 0)
                {
                    SendMessage(u);
                }
            }
        }

        private void ScrollViewer_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            if (sv != null)
            {
                sv.ScrollToBottom();
                TabControlMain_SelectionChanged(this.TabControlMain, null);
            }
        }

        private void TextBox_KeyDown_2(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                TextBox tb = sender as TextBox;
                if (tb != null)
                {
                    Grid p = tb.Parent as Grid;
                    if (p != null)
                    {
                        Models.User u = p.DataContext as Models.User;
                        u.MsgData = tb.Text;
                        if (u != null && u.MsgData.Length > 0)
                        {
                            SendMessage(u);
                        }
                    }
                }
            }
        }
    }
}
